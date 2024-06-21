using Trendsetter_Threads.API.Data.Entity;
using Trendsetter_Threads.API.Data.Entity.DbSet;
using Trendsetter_Threads.API.Data.Models;
using Trendsetter_Threads.API.Helper;
using Trendsetter_Threads.API.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Trendsetter_Threads.API.Repository.Service;
public class UserService : IUserService
{
    private readonly TrendsetterDbContext _db;
    private readonly IConfiguration _configuration;

    public UserService(TrendsetterDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<OperationResult> RegisterUser(UserModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Username))
            {
                return new OperationResult(false, "Email and Username are required fields.", StatusCodes.Status400BadRequest);
            }

            var userExists = await _db.Users.FirstOrDefaultAsync(x => x.Username == model.Username || x.Email == model.Email);
            if (userExists != null)
            {
                return new OperationResult(false, "Username or Email address has already been registered.", StatusCodes.Status409Conflict);
            }

            var newUser = new User
            {
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                Username = model.Username,
                Password = AesEncryption.Encrypt(model.Password),
                IsAdmin = model.IsAdmin
            };

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            return new OperationResult(true, "User created successfully.", StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            return new OperationResult(false, "Failed to register user. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<OperationResult<UserModel>> Login(LoginModel model)
    {
        var result = new OperationResult<UserModel>();

        try
        {
            if (model == null)
            {
                result.Message = "Invalid input data.";
                result.StatusCode = StatusCodes.Status400BadRequest;
                result.IsSuccess = false;
                return result;
            }

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == model.Username);

            if (user == null)
            {
                result.Message = "User not found.";
                result.StatusCode = StatusCodes.Status404NotFound;
                result.IsSuccess = false;
                return result;
            }

            var encryptedPassword = AesEncryption.Encrypt(model.Password);
            var validUser = await _db.Users.FirstOrDefaultAsync(x => x.Username == model.Username && x.Password == encryptedPassword);

            if (validUser == null)
            {
                result.Message = "Invalid password.";
                result.StatusCode = StatusCodes.Status401Unauthorized;
                result.IsSuccess = false;
                return result;
            }

            // Generate JWT token
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            var tokenExpiryTime = DateTime.UtcNow.AddHours(1); // Assuming UTC time for token expiry
            var token = GetToken(authClaims, tokenExpiryTime);

            // Generate refresh token (if needed)
            var refreshToken = GenerateRefreshToken();

            result.IsSuccess = true;
            result.StatusCode = StatusCodes.Status200OK;
            result.Message = "Login successful.";
            result.Data = new UserModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpiryTime = tokenExpiryTime.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Format token expiry time as ISO 8601
                Email = user.Email,
                Id = user.Id,
                Username = user.Username,
                IsAdmin = user.IsAdmin
            };

            return result;
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            result.Message = "Something went wrong! Please try again after some time.";
            result.StatusCode = StatusCodes.Status500InternalServerError;
            result.IsSuccess = false;
            return result;
        }
    }


    private JwtSecurityToken GetToken(List<Claim> authClaims, DateTime tokenExpiryTime)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: tokenExpiryTime,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber);
    }



}
