using DotNetAdditional.Contexts;
using DotNetAdditional.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetAdditional.Services;

public class UserService:IUserService
{
    private readonly DBContext _dbContext;

    public UserService(DBContext context)
    {
        _dbContext = context;
    }
    public async Task<User[]> GetUsers(int page)
    {
      
        var query = _dbContext.User
            .AsQueryable(); 
        return await query
            .Skip((page - 1) * 10)
            .Take(10)
            .ToArrayAsync();
    }
    public async Task<bool> DeleteUser(int id)
    { 
        await _dbContext.User
            .Where(p =>  p.Id == id)
            .ExecuteDeleteAsync();
        return true;
    }
}