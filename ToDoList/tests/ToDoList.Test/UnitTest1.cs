namespace ToDoList.Test;

public class CalculatorTests
{
    [Fact]
    public void Calculator_Add_ShouldReturnCorrectResult()
    {
        // Arrange
        var calculator = new Calculator();
        int a = 5;
        int b = 4;

        // Act
        int result = calculator.Add(a, b);

        // Assert
        Assert.Equal(9, result);

    }

    [Fact]
    public void Calculator_Divide_ShouldReturnCorrectResult()
    {
        // Arrange
        var calculator = new Calculator();
        int a = 10;
        int b = 2;

        // Act
        int result = calculator.Divide(a, b);

        // Assert
        Assert.Equal(5, result);

    }


}

public class Calculator
{
    public int Add(int a, int b)
    {
        return a + b;
    }
    public int Divide(int a, int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }
        return a / b;
    }
}
