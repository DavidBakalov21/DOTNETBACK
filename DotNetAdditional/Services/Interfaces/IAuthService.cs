namespace DotNetAdditional.Services;

public interface IAuthService
{
    public Task<(string, string)> Register(string username, string password, string email);
    
    public Task<(string, string)> Login(string email, string password);
    
    public Task<(string, string)> Refresh(string email);
}