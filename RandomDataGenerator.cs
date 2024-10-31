

namespace PTMKtask
{
    public class RandomDataGenerator
{
    private readonly Random _random = new Random();

    private readonly string[] _fullNames;

    public RandomDataGenerator()
    {
        if(!File.Exists("names.txt")) throw new FileNotFoundException("File 'names.txt' not found.");
        _fullNames= File.ReadAllLines("names.txt");
    }


    private string GenerateRandomFullName()
    {
        return _fullNames[_random.Next(_fullNames.Length)];
    }

    private DateOnly GenerateRandomBirthDate()
    {
        return new DateOnly(
            _random.Next(1950, 2006),
            _random.Next(1, 13),
            _random.Next(1, 29)); // Простая реализация, без учета дней в месяце
    }

    public Employee GenerateRandomEmployee()
    {
        return new Employee
        (
            $"{this.GenerateRandomFullName()}",
            this.GenerateRandomBirthDate(),
            _random.Next(2) == 0 ? Gender.Male : Gender.Female
        );
    }
}

}