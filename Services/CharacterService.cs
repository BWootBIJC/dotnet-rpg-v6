using System.Collections;
using AutoMapper;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services;

public class CharacterService : ICharacterService
{ 
    private readonly IMapper _mapper;
    private readonly RpgContext _dbContext;
    public CharacterService(IMapper mapper, RpgContext context)
    {
        _mapper = mapper;
        _dbContext = context;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        try
        {
            Character character = _mapper.Map<Character>(newCharacter);
            _dbContext.Characters.Add(character);
            await _dbContext.SaveChangesAsync();
            serviceResponse.Data = await _dbContext.Characters
                .Select(x => _mapper.Map<GetCharacterDto>(x))
                .ToListAsync();
            serviceResponse.Success = true;
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
        try
        {
            var character = await _dbContext.Characters.FirstOrDefaultAsync(x => x.Id == updatedCharacter.Id);

            await _dbContext.SaveChangesAsync();
            
            _mapper.Map(updatedCharacter, character);
            response.Data = _mapper.Map<GetCharacterDto>(character);
            response.Success = true;
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;

    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int userId)
    {
        var response = new ServiceResponse<List<GetCharacterDto>>();

        try
        {
            var dbCharacters = await _dbContext.Characters
                .Where(x => x.User.Id == userId)
                .ToListAsync();
            response.Data = dbCharacters.Select(x => _mapper.Map<GetCharacterDto>(x)).ToList();
            response.Success = true;
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }
        return response;

    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        var serviceResponse = new ServiceResponse<GetCharacterDto>();

        try
        {
            var dbCharacter = await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            serviceResponse.Success = true;
        }
        catch (Exception e)
        {
            serviceResponse.Message = e.Message;
            serviceResponse.Success = false;
        }
        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();

        try
        {
            var character = await _dbContext.Characters.FirstAsync(x => x.Id == id);
            _dbContext.Characters.Remove(character);
            await _dbContext.SaveChangesAsync();
            response.Data = _dbContext.Characters.Select(x => _mapper.Map<GetCharacterDto>(x)).ToList();
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
}