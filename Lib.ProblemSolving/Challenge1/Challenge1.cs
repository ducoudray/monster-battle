namespace Lib.ProblemSolving;

public static class Challenge1
{
    public static Challenge1Result FractionsCalculator(int[] numbers)
    {
        var positives = 0; 
        var negatives = 0;
        var zeros = 0;

        foreach (var number in numbers)
        {
            if (number > 0)
            {
                positives++;
            }
            else if (number < 0)
            {
                negatives++;
            }
            else
            {
                zeros++;
            }
        }

        var totalCountNumbers = numbers.Length;

        return new Challenge1Result()
        {
            Positives = Math.Round((decimal)positives / totalCountNumbers, 6),
            Negatives = Math.Round((decimal)negatives / totalCountNumbers,6),
            Zeros = Math.Round((decimal)zeros / totalCountNumbers,6),
        };
    }
}

public class Challenge1Result
{
    public decimal Positives { get; set; }
    public decimal Negatives { get; set; }
    public decimal Zeros { get; set; }
}