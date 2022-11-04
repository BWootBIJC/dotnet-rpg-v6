using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Repos;

public class AuthRepository : IAuthRepository
{
    private readonly RpgContext _dbContext;
    
    public AuthRepository(RpgContext dbContext)
    {
        _dbContext = dbContext;
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

    public Task<ServiceResponse<string>> Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}