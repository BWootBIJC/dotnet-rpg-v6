﻿using System.Security.Claims;
using DefaultNamespace.Dtos.Skills;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
    {
        return Ok(await _characterService.GetAllCharacters());
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id)
    {
        return Ok(await _characterService.GetCharacterById(id));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<List<Character>>>> AddCharacter(AddCharacterDto newCharacter)
    {
        return Ok(await _characterService.AddCharacter(newCharacter));
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(
        UpdateCharacterDto updatedCharacter)
    {
        var response = await _characterService.UpdateCharacter(updatedCharacter);
        if (response.Data == null)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Delete(int id)
    {
        var response = await _characterService.DeleteCharacter(id);
        if (response.Data == null)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost("Skill")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddCharacterSkill(
        AddCharacterSkillDto characterSkillDto)
    {
        return Ok(await _characterService.AddCharacterSkill(characterSkillDto));
    }
}