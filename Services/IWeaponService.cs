using DefaultNamespace.Dtos.Weapon;
using dotnet_rpg.Dtos.Character;

namespace dotnet_rpg.Services;

public interface IWeaponService
{
    Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
}