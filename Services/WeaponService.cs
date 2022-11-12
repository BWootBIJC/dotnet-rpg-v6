using System.Security.Claims;
using AutoMapper;
using DefaultNamespace.Dtos.Weapon;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services;

public class WeaponService : IWeaponService
{
    private readonly RpgContext _rpgContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;


    public WeaponService(RpgContext rpgContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _rpgContext = rpgContext;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }
    
    public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
    {
        ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
        try
        {
            Character character = await _rpgContext.Characters
                .FirstOrDefaultAsync(x => x.Id == newWeapon.CharacterId && 
                                          x.User!.Id == int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (character == null)
            {
                response.Success = false;
                response.Message = "Character not found.";
                return response;
            }

            Weapon weapon = new Weapon
            {
                Name = newWeapon.Name,
                Damage = newWeapon.Damage,
                Character = character
            };

            _rpgContext.Weapons.Add(weapon);
            await _rpgContext.SaveChangesAsync();
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
}