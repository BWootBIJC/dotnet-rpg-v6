namespace dotnet_rpg.Services;

public class UserService : IUserService
{
    private readonly IUserQueries _userQueries;

    public UserService(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    public Task<ServiceResponse<List<User>>> ListUsers()
    {
        throw new NotImplementedException();
    }
}