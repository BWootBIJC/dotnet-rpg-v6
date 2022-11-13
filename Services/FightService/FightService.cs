using AutoMapper;
using DefaultNamespace.Dtos.Fight;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.FightService;

public class FightService : IFightService
{
    private readonly RpgContext _dbContext;
    private readonly IMapper _mapper;

    public FightService(RpgContext dbcontext, IMapper mapper)
    {
        _dbContext = dbcontext;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
    {
        var response = new ServiceResponse<AttackResultDto>();

        try
        {
            var attacker = await _dbContext.Characters
                .Include(x => x.Weapon)
                .FirstOrDefaultAsync(x => x.Id == request.AttackerId);
            var opponent = await _dbContext.Characters
                .FirstOrDefaultAsync(x => x.Id == request.OpponentId);

            int damage = WeaponAttack(attacker, opponent);

            if (opponent.HitPoints <= 0)
            {
                response.Message = $"{opponent.Name} has been defeated!";
            }

            await _dbContext.SaveChangesAsync();

            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name,
                Opponent = opponent.Name,
                AttackerHP = attacker.HitPoints,
                OpponentHP = opponent.HitPoints,
                Damage = damage
            };
            response.Success = true;
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
    {
        var response = new ServiceResponse<AttackResultDto>();

        try
        {
            var attacker = await _dbContext.Characters
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(x => x.Id == request.AttackerId);
            var opponent = await _dbContext.Characters
                .FirstOrDefaultAsync(x => x.Id == request.OpponentId);
            var skill = attacker!.Skills.FirstOrDefault(x => x.Id == request.SkillId);

            if (skill == null)
            {
                response.Success = false;
                response.Message = $"{attacker.Name} doesn't know that skill";
            }

            int damage = SkillAttack(attacker, opponent, skill);

            if (opponent.HitPoints <= 0)
            {
                response.Message = $"{opponent.Name} has been defeated!";
            }

            await _dbContext.SaveChangesAsync();

            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name,
                Opponent = opponent.Name,
                AttackerHP = attacker.HitPoints,
                OpponentHP = opponent.HitPoints,
                Damage = damage
            };
            response.Success = true;
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
    {
        var response = new ServiceResponse<FightResultDto>
        {
            Data = new FightResultDto()
        };

        try
        {
            var characters = await _dbContext.Characters
                .Include(x => x.Weapon)
                .Include(x => x.Skills)
                .Where(x => request.CharacterIds.Contains(x.Id)).ToListAsync();

            bool defeated = false;
            while (!defeated)
            {
                foreach (Character attacker in characters)
                {
                    var opponents = characters.Where(x => x.Id != attacker.Id).ToList();
                    var opponentsCopy = opponents[new Random().Next(opponents.Count)];

                    int damage = 0;
                    string attackUsed = string.Empty;

                    bool useWeapon = new Random().Next(2) == 0;
                    if (useWeapon)
                    {
                        attackUsed = attacker.Weapon.Name;
                        damage = WeaponAttack(attacker, opponentsCopy);
                    }
                    else
                    {
                        var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                        attackUsed = skill.Name;
                        damage = SkillAttack(attacker, opponentsCopy, skill);
                    }
                    response.Data.Log
                        .Add($"{attacker.Name} attacks {opponentsCopy.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");
                    response.Success = true;

                    if (opponentsCopy.HitPoints <= 0)
                    {
                        defeated = true;
                        attacker.Victories++;
                        opponentsCopy.Losses++;
                        response.Data.Log.Add($"{opponentsCopy.Name} has been defeated!");
                        response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                        break;
                    }
                }
            }
            
            characters.ForEach(x =>
            {
                x.Fights++;
                x.HitPoints = 100;
            });
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<List<HighscoreDto>>> GetHighscore()
    {
        var response = new ServiceResponse<List<HighscoreDto>>();
        var characters = await _dbContext.Characters
            .Where(x => x.Fights > 0)
            .OrderByDescending(x => x.Victories)
            .ThenBy(x => x.Losses)
            .ToListAsync();
        
        try
        {
            response.Data = characters.Select(x => _mapper.Map<HighscoreDto>(x)).ToList();
            response.Success = true;
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    private static int WeaponAttack(Character? attacker, Character? opponent)
    {
        int damage = attacker!.Weapon.Damage + (new Random().Next(attacker.Strength));
        damage -= new Random().Next(opponent!.Defense);

        if (damage > 0)
        {
            opponent.HitPoints -= damage;
        }

        return damage;
    }

    private static int SkillAttack(Character? attacker, Character? opponent, Skill? skill)
    {
        int damage = skill!.Damage + (new Random().Next(attacker.Intelligence));
        damage -= new Random().Next(opponent!.Defense);

        if (damage > 0)
        {
            opponent.HitPoints -= damage;
        }

        return damage;
    }
}