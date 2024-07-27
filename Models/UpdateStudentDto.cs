namespace EmployeeAdminPortal.Models;

public class UpdateStudentDto
{
    public required string Name { get; set; }
    public required string School { get; set; }
    public string? Grade { get; set; }
}