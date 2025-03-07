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
    public List<Users> GetUsers()
    {
        return _dbContext.Users.ToList();  
    }
}