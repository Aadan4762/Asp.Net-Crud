namespace EmployeeAdminPortal.Models.Entities;

public class AddStudentDto
{
    public required string Name { get; set; }
    public required string School { get; set; }
    public string? Grade { get; set; }
}