namespace EmployeeAdminPortal.Dtos;

public class AuthServiceResponseDto
{
    public bool IsSucceed { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Message { get; set; }

    
}