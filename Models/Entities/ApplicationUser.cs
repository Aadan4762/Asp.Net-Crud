using Microsoft.AspNetCore.Identity;

namespace EmployeeAdminPortal.Core;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}