using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeAdminPortal.Dtos;
using EmployeeAdminPortal.Interfaces;
using EmployeeAdminPortal.Models.Entities;
using EmployeeAdminPortal.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StaticUserRoles = EmployeeAdminPortal.Models.Entities.StaticUserRoles;

namespace EmployeeAdminPortal.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new AuthServiceResponseDto
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var accessToken = GenerateNewJsonWebToken(authClaims, 1); // Access token with 1 hour expiry
            var refreshToken = GenerateNewJsonWebToken(authClaims, 24 * 7); // Refresh token with 7 days expiry

            return new AuthServiceResponseDto
            {
                IsSucceed = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Message = "Login successful"
            };
        }
        
        public async Task<AuthServiceResponseDto> RefreshTokenAsync(TokenRefreshDto tokenRefreshDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            try
            {
                var principal = tokenHandler.ValidateToken(tokenRefreshDto.RefreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    IssuerSigningKey = authSecret,
                    ClockSkew = TimeSpan.Zero // Remove delay of token when expire
                }, out validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return new AuthServiceResponseDto
                    {
                        IsSucceed = false,
                        Message = "Invalid token"
                    };

                // Generate new access token
                var authClaims = principal.Claims;
                var newAccessToken = GenerateNewJsonWebToken(authClaims, 1); // Access token with 1 hour expiry

                return new AuthServiceResponseDto
                {
                    IsSucceed = true,
                    AccessToken = newAccessToken,
                    Message = "Token refreshed successfully"
                };
            }
            catch (Exception ex)
            {
                return new AuthServiceResponseDto
                {
                    IsSucceed = false,
                    Message = "Invalid token"
                };
            }
        }
        

        public async Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto
                {
                    IsSucceed = false,
                    Message = "Invalid User name"
                };
            }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return new AuthServiceResponseDto
            {
                IsSucceed = true,
                Message = "User is now an ADMIN"
            };
        }

        public async Task<AuthServiceResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto
                {
                    IsSucceed = false,
                    Message = "Invalid User name"
                };
            }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

            return new AuthServiceResponseDto
            {
                IsSucceed = true,
                Message = "User is now an OWNER"
            };
        }

        public async Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);

            if (isExistsUser != null)
            {
                return new AuthServiceResponseDto
                {
                    IsSucceed = false,
                    Message = "UserName Already Exists"
                };
            }

            var newUser = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User Creation Failed Because: " + string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                return new AuthServiceResponseDto
                {
                    IsSucceed = false,
                    Message = errorString
                };
            }

            // Add a Default USER Role to all users
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

            return new AuthServiceResponseDto
            {
                IsSucceed = true,
                Message = "User Created Successfully"
            };
        }

        public async Task<AuthServiceResponseDto> SeedRolesAsync()
        {
            bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (isOwnerRoleExists && isAdminRoleExists && isUserRoleExists)
            {
                return new AuthServiceResponseDto
                {
                    IsSucceed = true,
                    Message = "Roles Seeding is Already Done"
                };
            }

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

            return new AuthServiceResponseDto
            {
                IsSucceed = true,
                Message = "Role Seeding Done Successfully"
            };
        }

        private string GenerateNewJsonWebToken(IEnumerable<Claim> claims, int hours)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(hours),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenObject);
        }
    }
}
