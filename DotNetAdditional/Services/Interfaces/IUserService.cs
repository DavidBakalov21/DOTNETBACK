using DotNetAdditional.Entities;

namespace DotNetAdditional.Services;

public interface IUserService
{
    public Task<User[]> GetUsers(int page);
    public Task<bool> DeleteUser(int id);
}