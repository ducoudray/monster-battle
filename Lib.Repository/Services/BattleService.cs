using Lib.Repository.Entities;

namespace Lib.Repository.Services;

public static class BattleService
{
    public static Monster Monster(Monster monsterA, Monster monsterB)
    {
        Monster firstAttacker;
        if (monsterA.Speed > monsterB.Speed)
        {
            firstAttacker = monsterA;
        }
        else if (monsterA.Speed == monsterB.Speed)
        {
            firstAttacker = monsterA.Attack >= monsterB.Attack ? monsterA : monsterB;
        }
        else
        {
            firstAttacker = monsterB;
        }

        var secondAttacker = firstAttacker == monsterA ? monsterB : monsterA;

        while (monsterA.Hp > 0 && monsterB.Hp > 0)
        {
            var damage = Math.Max(firstAttacker.Attack - secondAttacker.Defense, 1);
            secondAttacker.Hp -= damage;

            (firstAttacker, secondAttacker) = (secondAttacker, firstAttacker);
        }

        var winner = monsterA.Hp > 0 ? monsterA : monsterB;
        return winner;
    }
}