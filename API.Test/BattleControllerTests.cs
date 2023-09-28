using API.Controllers;
using API.Test.Fixtures;
using FluentAssertions;
using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Test;

public class BattleControllerTests
{
    private readonly Mock<IBattleOfMonstersRepository> _repository;

    public BattleControllerTests()
    {
        this._repository = new Mock<IBattleOfMonstersRepository>();
    }

    [Fact]
    public async void Get_OnSuccess_ReturnsListOfBattles()
    {
        this._repository
            .Setup(x => x.Battles.GetAllAsync())
            .ReturnsAsync(BattlesFixture.GetBattlesMock());

        BattleController sut = new BattleController(_repository.Object);
        ActionResult result = await sut.GetAll();
        OkObjectResult objectResults = (OkObjectResult) result;
        objectResults?.Value.Should().BeOfType<Battle[]>();
    }
    
    [Fact]
    public async Task Post_BadRequest_When_StartBattle_With_nullMonster()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        
        Battle b = new Battle()
        {
            MonsterA = null,
            MonsterB = monstersMock[1].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));

        int? idMonsterA = null;
        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(() => null);

        int? idMonsterB = monstersMock[1].Id;
        Monster monsterB = monstersMock[1];

        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        BadRequestObjectResult objectResults = (BadRequestObjectResult) result;
        result.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal("Missing ID", objectResults.Value);
    }
    
    [Fact]
    public async Task Post_OnNoMonsterFound_When_StartBattle_With_NonexistentMonster()
    {
        //Arrange 
        var b = new Battle()
        {
            MonsterA = 1,
            MonsterB = 123
        };

        _repository.Setup(x => x.Battles.AddAsync(b));
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterB)).ReturnsAsync(() => null);
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterA)).ReturnsAsync(() => new Monster(){Id = b.MonsterA});

        var battleController = new BattleController(_repository.Object); 

        //act 
        var result = await battleController.Add(b);
        var objectResults = (BadRequestObjectResult) result;

        //Assert
        objectResults.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal("Invalid Monster ID", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning()
    {
        //arrange 
        var monsterA = new Monster()
        {
            Id = 1,
            Name = "Monster A",
            Attack = 50,
            Defense = 30,
            Speed = 70,
            Hp = 100
        };

        var monsterB = new Monster()
        {
            Id = 2,
            Name = "Monster B",
            Attack = 40,
            Defense = 40,
            Speed = 60,
            Hp = 80
        };

        var b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        _repository.Setup(x => x.Battles.AddAsync(b));
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterA)).ReturnsAsync(() => monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterB)).ReturnsAsync(() => monsterB);

        var battleController = new BattleController(_repository.Object);
        var result = await battleController.Add(b);
        var objectResults = (OkObjectResult) result;
        var winner = (Monster) objectResults.Value!;

        //Assert 
        objectResults.Should().BeOfType<OkObjectResult>();
        winner.Should().Be(monsterA);

    }


    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning()
    {
        //arrange 
        var monsterA = new Monster()
        {
            Id = 1,
            Name = "Monster A",
            Attack = 40,
            Defense = 30,
            Speed = 50,
            Hp = 100
        };

        var monsterB = new Monster()
        {
            Id = 2,
            Name = "Monster B",
            Attack = 60,
            Defense = 40,
            Speed = 70,
            Hp = 80
        };

        var b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        _repository.Setup(x => x.Battles.AddAsync(b));
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterA)).ReturnsAsync(() => monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterB)).ReturnsAsync(() => monsterB);

        var battleController = new BattleController(_repository.Object);
        var result = await battleController.Add(b);
        var objectResults = (OkObjectResult)result;
        var winner = (Monster)objectResults.Value!;

        //Assert 
        objectResults.Should().BeOfType<OkObjectResult>();
        winner.Should().Be(monsterB);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirSpeedsSame_And_MonsterA_Has_Higher_Attack()
    {
        //arrange 
        var monsterA = new Monster()
        {
            Id = 1,
            Name = "Monster A",
            Attack = 70,
            Defense = 30,
            Speed = 70,
            Hp = 100
        };

        var monsterB = new Monster()
        {
            Id = 2,
            Name = "Monster B",
            Attack = 60,
            Defense = 40,
            Speed = 70,
            Hp = 80
        };

        var b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        _repository.Setup(x => x.Battles.AddAsync(b));
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterA)).ReturnsAsync(() => monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterB)).ReturnsAsync(() => monsterB);

        var battleController = new BattleController(_repository.Object);
        var result = await battleController.Add(b);
        var objectResults = (OkObjectResult)result;
        var winner = (Monster)objectResults.Value!;

        //Assert 
        objectResults.Should().BeOfType<OkObjectResult>();
        winner.Should().Be(monsterA);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning_When_TheirSpeedsSame_And_MonsterB_Has_Higher_Attack()
    {
        //arrange 
        var monsterA = new Monster()
        {
            Id = 1,
            Name = "Monster A",
            Attack = 50,
            Defense = 30,
            Speed = 70,
            Hp = 100
        };

        var monsterB = new Monster()
        {
            Id = 2,
            Name = "Monster B",
            Attack = 70,
            Defense = 40,
            Speed = 70,
            Hp = 80
        };

        var b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        _repository.Setup(x => x.Battles.AddAsync(b));
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterA)).ReturnsAsync(() => monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterB)).ReturnsAsync(() => monsterB);

        var battleController = new BattleController(_repository.Object);
        var result = await battleController.Add(b);
        var objectResults = (OkObjectResult)result;
        var winner = (Monster)objectResults.Value!;

        //Assert 
        objectResults.Should().BeOfType<OkObjectResult>();
        winner.Should().Be(monsterB);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirDefensesSame_And_MonsterA_Has_Higher_Speed()
    {
        //arrange 
        var monsterA = new Monster()
        {
            Id = 1,
            Name = "Monster A",
            Attack = 60,
            Defense = 70,
            Speed = 80,
            Hp = 100
        };

        var monsterB = new Monster()
        {
            Id = 2,
            Name = "Monster B",
            Attack = 60,
            Defense = 70,
            Speed = 70,
            Hp = 80
        };

        var b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        _repository.Setup(x => x.Battles.AddAsync(b));
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterA)).ReturnsAsync(() => monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(b.MonsterB)).ReturnsAsync(() => monsterB);

        var battleController = new BattleController(_repository.Object);
        var result = await battleController.Add(b);
        var objectResults = (OkObjectResult)result;
        var winner = (Monster)objectResults.Value!;

        //Assert 
        objectResults.Should().BeOfType<OkObjectResult>();
        winner.Should().Be(monsterA);
    }

    [Fact]
    public async Task Delete_OnSuccess_RemoveBattle()
    {
        //Arrange 
        var id = 1;
        var battle = BattlesFixture.GetBattlesMock().ToArray();

        _repository.Setup(x => x.Battles.FindAsync(id)).ReturnsAsync(() => battle[0]);
        _repository.Setup(x => x.Battles.RemoveAsync(id));

        var battleController = new BattleController(_repository.Object);
        //Act 
        var result = await battleController.Remove(id);
        var objectResults = (OkResult)result;
        
        //Assert 
        objectResults.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_OnNoBattleFound_Returns404()
    {
        //Arrange 
        var id = 1;

        _repository.Setup(x => x.Battles.FindAsync(id)).ReturnsAsync(() => null);
        _repository.Setup(x => x.Battles.RemoveAsync(id));

        var battleController = new BattleController(_repository.Object);
        //Act 
        var result = await battleController.Remove(id);
        var objectResults = (NotFoundObjectResult)result;

        //Assert 
        objectResults.Should().BeOfType<NotFoundObjectResult>();
        Assert.Equal($"The battle with ID = {id} not found.", objectResults.Value);
    }
}
