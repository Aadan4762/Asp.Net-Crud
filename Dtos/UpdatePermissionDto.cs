using System.ComponentModel.DataAnnotations;

namespace EmployeeAdminPortal.Dtos;

public class UpdatePermissionDto
{
    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; } 
      
}