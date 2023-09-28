using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Lib.Repository.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class BattleController : BaseApiController
{
    private readonly IBattleOfMonstersRepository _repository;

    public BattleController(IBattleOfMonstersRepository repository)
    {
        _repository = repository;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll()
    {
        IEnumerable<Battle> battles = await _repository.Battles.GetAllAsync();
        return Ok(battles);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Add([FromBody] Battle battle)
    {
        if (battle.MonsterA == null || battle.MonsterB == null)
        {
            return BadRequest("Missing ID");
        }

        var monsterA = await _repository.Monsters.FindAsync(battle.MonsterA.Value);
        var monsterB = await _repository.Monsters.FindAsync(battle.MonsterB.Value);

        if (monsterA == null || monsterB == null)
        {
            return BadRequest("Invalid Monster ID");
        }

        var winner = BattleService.Monster(monsterA, monsterB);
        battle.Winner = winner.Id;

        await _repository.Battles.AddAsync(battle);
        await _repository.Save(); 

        return Ok(winner);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Remove(int id)
    {
        var battle = await _repository.Battles.FindAsync(id);

        if (battle == null)
        {
            return new NotFoundObjectResult($"The battle with ID = {id} not found.");
        }

        await _repository.Battles.RemoveAsync(id);
        await _repository.Save();
        return Ok();
    }
}


