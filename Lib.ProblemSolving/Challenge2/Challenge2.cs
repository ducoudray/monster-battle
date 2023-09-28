namespace Lib.ProblemSolving;

public static class Challenge2
{
    public static int DiceFacesCalculator(int dice1, int dice2, int dice3)
    {
        if (dice1 is < 1 or > 6 || dice2 is < 1 or > 6 || dice3 is < 1 or > 6)
        {
            throw new Exception("Dice out of number range");
        }

        if (dice1 == dice2 && dice2 == dice3)
        {
            return dice1 * 3;
        }

        if ((dice1 == dice2 && dice1 != dice3) || (dice1 == dice3 && dice1 != dice2))
        {
            return dice1 * 2;
        }

        if ((dice1 != dice2 && dice1 != dice3) || (dice2 != dice3 && dice1 != dice2))
        {
            int[] dicesFaces = {dice1, dice2, dice3};
            return dicesFaces.Max();
        }

        return 0;
    }
}