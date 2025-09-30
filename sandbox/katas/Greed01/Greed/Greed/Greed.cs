using System;

namespace Greed;

public class Greed
{
    public int score(int[] dice)
    {
        var score = 0;
        var counts = new int[7]; // Index 0 will be unused, we'll use 1-6

        // 3, 4, 5, 6, 1
        foreach (var die in dice)
        {
            counts[die]++;
        }

        // Score triples
        if (counts[1] >= 3)
        {
            score += 1000;
            // avoid double counting
            counts[1] -= 3;
        }
        for (int i = 2; i <= 6; i++)
        {
            if (counts[i] >= 3)
            {
                score += i * 100;
                counts[i] -= 3;
            }
        }

        // Score remaining singles
        score += counts[1] * 100; // For each 1
        score += counts[5] * 50;  // For each 5

        return score;
    }

    public int[] rollDice()
    {
        Random random = new Random();
        int[] dice = new int[5];
        for (int i = 0; i < dice.Length; i++)
        {
            dice[i] = random.Next(1, 7); // Generuje čísla od 1 do 6
        }
        return dice;
    }
}
