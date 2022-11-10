using AutoMapper;
using dotnet_rpg;
using dotnet_rpg.Dtos.User;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.Configuration;

public class UserQueries : IUserQueries
{
    private readonly RpgContext _dbContext;
    private readonly IMapper _mapper;


    public UserQueries(RpgContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    // public async Task<ServiceResponse<List<User>>> ListUsers()
    // {
    //     var response = new ServiceResponse<List<UserDto>>();
    //     var result = await _dbContext.Users.ToListAsync();
    //     var userDto = result
    //     response.Success = true;
    //     return response;
    // }
}