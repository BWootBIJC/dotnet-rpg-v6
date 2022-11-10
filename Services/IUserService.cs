namespace dotnet_rpg.Services;

public interface IUserService
{
    Task<ServiceResponse<List<User>>> ListUsers();
}