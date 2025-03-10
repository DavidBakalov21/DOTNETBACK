using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetAdditional.Contexts;
using DotNetAdditional.Entities;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DotNetAdditional.Services;

public class AuthService:IAuthService
{
    private readonly DBContext _dbContext;
    private readonly string AT_SECRET;
    private readonly string RT_SECRET;


    public AuthService(DBContext dbContext)
    {
        _dbContext = dbContext;
        AT_SECRET = Environment.GetEnvironmentVariable("AT_SECRET");
        RT_SECRET = Environment.GetEnvironmentVariable("RT_SECRET");
        
    }

    public async Task<(string, string)> Register(string username, string password, string email)
    {
        if (await _dbContext.User.AnyAsync(u => u.Email == email))
        {
            return ("", "");
        }
        var hashedPassword = HashPassword(password);
       await _dbContext.User.AddAsync(new User
        {
            Username = username,
            Email = email,
            Password = hashedPassword,
            Role= "user"
        });
       await _dbContext.SaveChangesAsync();
        var at = GenerateAT(email, "user");
        var rt = GenerateRT(email, "user");
        return (at, rt);
    }

    public async Task<(string, string)> Login(string email, string password)
    {
        var user =await _dbContext.User.FirstAsync(u => u.Email == email);
        if (VerifyPassword(password,user.Password))
        {
         return (GenerateAT(email, user.Role), GenerateRT(email, user.Role));
        }
        return ("", "");
    }

    public async Task<(string, string)> Refresh(string email)
    {
        var user =await _dbContext.User.FirstAsync(u => u.Email == email);
        if (user is not null)
        {
            return (GenerateAT(email,user.Role), GenerateRT(email, user.Role));
        }
        return ("", "");
    }
    private string GenerateAT(string userEmail,string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AT_SECRET));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role,role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(2000),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRT(string userEmail,string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RT_SECRET));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role,role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10000),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}
