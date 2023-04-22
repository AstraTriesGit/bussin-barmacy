using MySql.Data.MySqlClient;

namespace Uhhh;

public class Pharma
{
    private static string adminUsername = "root";
    private static string adminPassword = "gg";
    private const string myConnectionString = "server=127.0.0.1;uid=root;pwd=420LMAOyeet69;database=pharmacy";

    public static MySqlConnection conn;
    
    public static void Main()
    {
        // database init
        try
        {
            conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();
        }
        catch(MySqlException ex)
        {
            Console.WriteLine("Death.");
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }
        
        bool run = true;
        string welcome = 
            @"Welcome to the pharmacy! 
Enter the input:
1) Login as customer.
2) Login as administrator.
3) Press 0 to exit the program.";
        Console.WriteLine(welcome);
        while (run)
        {

            var input = int.Parse(Console.ReadLine());
            switch (input)
            {
                case 1:
                    Console.WriteLine("Enter your username:");
                    var username = Console.ReadLine().Trim();
                    Console.WriteLine("Enter your password:");
                    var pwd = Console.ReadLine().Trim();
                    if (Verified("Customer", username, pwd))
                    {
                        Console.WriteLine("You're in, " + username + "!");
                        Customer customer = new Customer(username);
                        customer.CustomerMenu();
                    }
                    else
                    {
                        Console.WriteLine("Login failed!");
                    }
                    break;
                case 2:
                    Console.WriteLine("Enter your username");
                    username = Console.ReadLine().Trim();
                    Console.WriteLine("Enter your password");
                    pwd = Console.ReadLine().Trim();
                    if (Verified("Admin", username, pwd))
                    {
                        Console.WriteLine("You're in, " + username + "!");
                        Admin admin = new Admin();
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
        switch (mode)
        {
            case "Admin":
                return (username == adminUsername) && (pwd == adminPassword);
            case "Customer":
                return pwd == Query.GetPassword(username);
                break;
            default:
                return false;
        }
        
    }
}