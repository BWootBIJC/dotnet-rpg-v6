using AutoMapper;
using DefaultNamespace.Dtos.Fight;
using DefaultNamespace.Dtos.Skill;
using DefaultNamespace.Dtos.Weapon;
using dotnet_rpg.Dtos.Character;

namespace DefaultNamespace;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Character, GetCharacterDto>();
        CreateMap<AddCharacterDto, Character>();
        CreateMap<UpdateCharacterDto, Character>();
        CreateMap<Weapon, GetWeaponDto>();
        CreateMap<Skill, GetSkillDto>();
        CreateMap<Character, HighscoreDto>();
    }
}