using EmployeeAdminPortal.Dtos;

namespace EmployeeAdminPortal.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResponseDto> SeedRolesAsync();
    Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto);
    Task<AuthServiceResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto);
}
