using DotNetAdditional.Contexts;
using DotNetAdditional.Entities;

namespace DotNetAdditional.Services;

public class UserService:IUserService
{
    private readonly DBContext _dbContext;

    public UserService(DBContext context)
    {
        _dbContext = context;
    }
    public List<User> GetUsers()
    {
        return _dbContext.User.ToList();  
    }
}