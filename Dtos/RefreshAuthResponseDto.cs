namespace EmployeeAdminPortal.Dtos;

public class RefreshAuthResponseDto
{
    public bool IsSucceed { get; set; }
    public string AccessToken { get; set; }
    public string Message { get; set; }
}