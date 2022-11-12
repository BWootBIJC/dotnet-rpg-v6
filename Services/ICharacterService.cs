using dotnet_rpg.Dtos.Character;

namespace dotnet_rpg.Services;

public interface ICharacterService
{
    Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int userId);
    Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);
    Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);
    Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacterDto);
    Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);
}