namespace FizzBuzz;

public class FizzBuzz
{
    public void CountTo(int lastNumber)
    {
        for (int actualNumber = 1; actualNumber < lastNumber; actualNumber++)
        {
            if (actualNumber % 3 == 0 && actualNumber % 5 == 0)
            {
                Console.WriteLine("FizzBuzz");
            }
            else if (actualNumber % 3 == 0)
            {
                Console.WriteLine("Fizz");
            }
            else if (actualNumber % 5 == 0)
            {
                Console.WriteLine("Buzz");
            }
            else
            {
                Console.WriteLine(actualNumber);
            }
        }

    }
}
