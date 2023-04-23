using MySql.Data.MySqlClient;

namespace Uhhh;

public static class Pharma
{
    private const string AdminUsername = "root";
    private const string AdminPassword = "gg";
    private const string MyConnectionString = "server=127.0.0.1;uid=root;pwd=youreallythoughtthiswasthepassword;database=pharmacy";

    public static MySqlConnection? Conn;
    
    public static void Main()
    {
        // database init
        try
        {
            Conn = new MySqlConnection();
            Conn.ConnectionString = MyConnectionString;
            Conn.Open();
        }
        catch(MySqlException ex)
        {
            Console.WriteLine("Death.");
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        const string welcome = @"Welcome to the pharmacy! 
Enter the input:
1) Login as customer.
2) Login as administrator.
3) Press 0 to exit the program.";
        while (true)
        {
            Console.WriteLine(welcome);

            var input = int.Parse(Console.ReadLine() ?? string.Empty);
            switch (input)
            {
                case 1:
                    Console.WriteLine("Enter your username:");
                    var username = Console.ReadLine()!.Trim();
                    Console.WriteLine("Enter your password:");
                    var pwd = Console.ReadLine()!.Trim();
                    if (Verified("Customer", username, pwd))
                    {
                        Console.WriteLine("You're in, " + username + "!");
                        var customer = new Customer(username);
                        customer.CustomerMenu();
                    }
                    else
                    {
                        Console.WriteLine("Login failed!");
                    }
                    break;
                case 2:
                    Console.WriteLine("Enter your username");
                    username = Console.ReadLine()!.Trim();
                    Console.WriteLine("Enter your password");
                    pwd = Console.ReadLine()!.Trim();
                    if (Verified("Admin", username, pwd))
                    {
                        Console.WriteLine("You're in, " + username + "!");
                        var admin = new Admin();
                        admin.AdminMenu();
                    }
                    else
                    {
                        Console.WriteLine("Login failed!");
                    }
                    break;
                case 0:
                    Console.WriteLine("Thank you for visiting us!");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Not a valid choice!");
                    break;
            }
        }
        
    }

    private static bool Verified(string mode, string username, string pwd)
    {
        return mode switch
        {
            "Admin" => (username == AdminUsername) && (pwd == AdminPassword),
            "Customer" => pwd == Query.GetPassword(username),
            _ => false
        };
    }
}