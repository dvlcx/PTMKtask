using Microsoft.Data.Sqlite;

namespace PTMKtask;

class Program
{
    static void Main(string[] args)
    {
        switch (args[0])
        {
            case "1":
                Console.WriteLine("Table creation mode.");
                CreateEmployeesTable();
                break;

            case "2":
                Console.WriteLine("Insert directly mode.");
                if(args.Length != 4)
                {
                    Console.WriteLine("Incorrect number of arguments");
                    break;
                }
                InsertDirectly(args[1], args[2], args[3]);
                break;

            case "3":
                Console.WriteLine("List all unique names+birthdays mode.");
                ListAllUnique();
                break;

            case "4": //займёт очень много времени, но в любой момент можно остановить
                Console.WriteLine("Fast population mode.");
                PopulateTable();
                break;

            case "5":
                Console.WriteLine("Filter mode.");
                Filter();
                break;
            default:
                Console.WriteLine("Such args are not supported");
                return;
        }

        Console.WriteLine("Press enter to exit...");
        Console.ReadLine();
    }
    

    private static void CreateEmployeesTable()
    {
        using (var connection = new SqliteConnection("Data Source=TaskDatabase.db"))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='employees';";
            SqliteDataReader reader =command.ExecuteReader();
            if(reader.HasRows)
            {
                reader.Close();
                Console.WriteLine("'employees' table already exists");
                return;
            }
            reader.Close();
            command.CommandText = "CREATE TABLE employees(id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, full_name TEXT NOT NULL, date_of_birth DATE NOT NULL, gender TEXT NOT NULL);";
            command.ExecuteNonQuery();
            Console.WriteLine("table 'employees' created");
        }
    }

    private static void InsertDirectly(string fullName, string dateOfBirth, string genderString)
    {
        Gender gender;
        Enum.TryParse(genderString, out gender);
        var newEmployee = new Employee( fullName, DateOnly.Parse(dateOfBirth), gender);
        using (var connection = new SqliteConnection("Data Source=TaskDatabase.db"))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = $"INSERT INTO employees(full_name, date_of_birth, gender) VALUES('{newEmployee.FullName}', '{newEmployee.DateOfBirth}', '{newEmployee.Gender}');";
            command.ExecuteNonQuery();
            Console.WriteLine("record inserted");
        }
    }

    private static void ListAllUnique()
    {
        using (var connection = new SqliteConnection("Data Source=TaskDatabase.db"))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM employees GROUP BY full_name, date_of_birth";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var employee = new Employee(reader.GetString(1), DateOnly.Parse(reader.GetString(2)), (Gender)reader.GetInt32(3));
                Console.WriteLine(employee.FullName + " " + employee.DateOfBirth + " " + employee.Gender + " " + employee.GetAge());
            }
            reader.Close();
        }
    }

    private static void PopulateTable()
    {
        var randomDataGenerator = new RandomDataGenerator();
        using (var connection = new SqliteConnection("Data Source=TaskDatabase.db"))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            for (int i = 0; i < 1000000; i++)
            {
                var emp = randomDataGenerator.GenerateRandomEmployee();
                command.CommandText = $"INSERT INTO employees(full_name, date_of_birth, gender) VALUES('{emp.FullName}', '{emp.DateOfBirth}', '{emp.Gender}');";
                command.ExecuteNonQuery();
            }
            Console.WriteLine("1000000 records inserted");
        }
    }

    private static void Filter()
    {

        using (var connection = new SqliteConnection("Data Source=TaskDatabase.db"))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM employees WHERE full_name LIKE 'F%' AND gender = 'Male';";
            var watch = System.Diagnostics.Stopwatch.StartNew();
            SqliteDataReader reader = command.ExecuteReader();
            watch.Stop();
            while (reader.Read())
            {
                var employee = new Employee(reader.GetString(1), DateOnly.Parse(reader.GetString(2)), (Gender)reader.GetInt32(3));
                Console.WriteLine(employee.FullName + " " + employee.DateOfBirth + " " + employee.Gender + " " + employee.GetAge());
            }
            reader.Close();
            Console.WriteLine("execution time: " + watch.Elapsed);
        }
    }
}