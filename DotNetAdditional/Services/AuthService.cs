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
        if (await _dbContext.Users.AnyAsync(u => u.Email == email))
        {
            return ("", "");
        }
        var hashedPassword = HashPassword(password);
       await _dbContext.Users.AddAsync(new Users
        {
            Username = username,
            Email = email,
            Password = hashedPassword
        });
       await _dbContext.SaveChangesAsync();
        var at = GenerateAT(email);
        var rt = GenerateRT(email);
        return (at, rt);
    }

    public async Task<(string, string)> Login(string email, string password)
    {
        var user =await _dbContext.Users.FirstAsync(u => u.Email == email);
        if (VerifyPassword(password,user.Password))
        {
         return (GenerateAT(email), GenerateRT(email));
        }
        return ("", "");
    }

    public async Task<(string, string)> Refresh(string email)
    {
        var user =await _dbContext.Users.FirstAsync(u => u.Email == email);
        if (user is not null)
        {
            return (GenerateAT(email), GenerateRT(email));
        }
        return ("", "");
    }
    private string GenerateAT(string userEmail)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AT_SECRET));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, userEmail)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRT(string userEmail)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RT_SECRET));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, userEmail)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}
