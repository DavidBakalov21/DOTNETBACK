using System.Text;
using DotNetAdditional.Contexts;
using DotNetAdditional.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication()
    .AddJwtBearer("Access", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,    
            ValidateAudience = false,  
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("AT_SECRET")))
        };
    })
    .AddJwtBearer("Refresh", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,      
            ValidateAudience = false,  
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("RT_SECRET")))
        };
    });
var connectionUrl = builder.Configuration.GetConnectionString("Connection");

var databaseUri = new Uri(connectionUrl);
var userInfo = databaseUri.UserInfo.Split(':');

var connectionString = new Npgsql.NpgsqlConnectionStringBuilder
{
    Host = databaseUri.Host,
    Port = databaseUri.Port,
    Username = userInfo[0],
    Password = userInfo[1],
    Database = databaseUri.AbsolutePath.TrimStart('/'),
    TrustServerCertificate = true
}.ToString();

builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
var app = builder.Build();


app.MapControllers();
app.UseAuthentication();  
app.UseAuthorization(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region API
app.MapGet("/", () => "Hello World!");
#endregion

app.Run();


public partial class Program
{
}