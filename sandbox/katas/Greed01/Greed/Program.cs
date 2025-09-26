// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, you!");
Console.WriteLine("Házím kostkou a padla mi tato čísla:");
Greed.Greed greed = new Greed.Greed();
int[] dice = greed.rollDice();
foreach (var die in dice)
{
    Console.Write(die + ", ");
}
Console.WriteLine();
Console.WriteLine("Moje skóre je: " + greed.score(dice));

