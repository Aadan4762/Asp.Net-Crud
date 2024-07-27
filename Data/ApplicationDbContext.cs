using EmployeeAdminPortal.Models.Entities;
using EmployeeAdminPortal.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StaticUserRoles = EmployeeAdminPortal.Models.Entities.StaticUserRoles;

namespace EmployeeAdminPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<ApplicationUser> Registers { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = StaticUserRoles.USER, NormalizedName = StaticUserRoles.USER.ToUpper() },
                new IdentityRole { Name = StaticUserRoles.ADMIN, NormalizedName = StaticUserRoles.ADMIN.ToUpper() },
                new IdentityRole { Name = StaticUserRoles.OWNER, NormalizedName = StaticUserRoles.OWNER.ToUpper() }
            );
        }
    }
}