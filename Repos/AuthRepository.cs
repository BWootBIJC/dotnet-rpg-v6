using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Repos;

public class AuthRepository : IAuthRepository
{
    private readonly RpgContext _dbContext;
    private readonly IConfiguration _configuration;
    
    public AuthRepository(RpgContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }
    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        ServiceResponse<int> response = new ServiceResponse<int>();
        if (await UserExists(user.Username))
        {
            response.Success = false;
            response.Message = "User already exists";
            return response;
        }
        try
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            response.Success = true;
            response.Message = "User successfully created";
            response.Data = user.Id;
            return response;

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UserExists(string username)
    {
        return await _dbContext.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower());
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
        var response = new ServiceResponse<string>();
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));

        if (user == null)
        {
            response.Success = false;
            response.Message = "User not found";
        }
        else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "Wrong Password.";
        }
        else
        {
            response.Success = true;
            response.Message = "Successfully authenticated";
            response.Data = CreateToken(user);
        }

        return response;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        {
            var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(passwordHash);
        }
    }
}