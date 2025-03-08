using DotNetAdditional.Entities;

namespace DotNetAdditional.Services;

public interface IUserService
{
    public List<User> GetUsers();
}