
namespace Uhhh;

public class Customer
{
    private string username;
    private int customerID;

    public Customer(string username)
    {
        this.username = username;
        customerID = Query.GetCustomerID(username);
    }

    public void CustomerMenu()
    {
        string menu = @"Select your choice:
1) View products
2) Add product to cart
3) Check cart
4) Checkout
5) View past orders
0) Exit
";
        bool run = true;
        while (run)
        {
            Console.WriteLine(menu);
            
            int input = int.Parse(Console.ReadLine().Trim());
            switch (input)
            {
                case 1:
                    ViewProducts();
                    break;
                case 2:
                    AddProducts();
                    break;
                case 3:
                    InspectCart();
                    break;
                case 4:
                    Checkout();
                    break;
                case 5:
                    ViewOrderHistory();
                    break;
                case 0:
                    Console.WriteLine("Visit us again!");
                    run = false;
                    break;
                default:
                    Console.WriteLine("Not a valid input!");
                    break;
            }
        }
    }

    private void ViewProducts()
    {
        bool run = true;
        int threshold = 0;
        string like = "%";
        while (run)
        {
            Console.WriteLine("Filter by price/name, see all or exit? (price/name/all/exit)");
            string input = Console.ReadLine().Trim();

            switch (input)
            {
                case "price":
                    Console.WriteLine("Enter the price to get products below that price:");
                    threshold = int.Parse(Console.ReadLine().Trim());
                    Query.ViewProducts(threshold, like);
                    break;
                case "name":
                    Console.WriteLine("Enter the name of the product you are searching for:");
                    like = Console.ReadLine().Trim();
                    Query.ViewProducts(threshold, like);
                    break;
                case "all":
                    Query.ViewProducts(0, "%");
                    break;
                case "exit":
                    run = false;
                    break;
                default:
                    Console.WriteLine("Not a valid input!");
                    break;
            }
            
        }
    }

    private void AddProducts()
    {
        Console.WriteLine("Enter the name of the product:");
        string name = Console.ReadLine().Trim();
        Console.WriteLine("Enter the quantity of the product to purchase:");
        int quantity = int.Parse(Console.ReadLine().Trim());
        Query.AddToCart(name, quantity, customerID);
        
        Console.WriteLine("Product added!");
    }

    private void InspectCart()
    {
        Console.WriteLine("Here are the items in your cart:");
        Query.InspectCart(customerID);
        Console.WriteLine("The total cost of your cart is: " + Query.CartCost(customerID));
    }

    private void Checkout()
    {
        if (Query.CartSanity(customerID))
        {
            Console.WriteLine("Enter your mode of payment(COD/Online):");
            string mode = Console.ReadLine().Trim();
            switch (mode)
            {
                case "COD":
                    Console.WriteLine("Success! Your order will be delivered soon.");
                    Query.ClearCart(customerID, mode);
                    break;
                case "Online":
                    Console.WriteLine("Waiting for payment confirmation...");
                    Thread.Sleep(3000);
                    Console.WriteLine("Success! Your order will be delivered soon.");
                    Query.ClearCart(customerID, mode);
                    break;
                default:
                    Console.WriteLine("Not a valid input!");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Cannot check out: product quantity in cart exceeds available quantity!");
        }
    }

    private void ViewOrderHistory()
    {
        Query.GetCustomerOrders(customerID);
        
        Console.WriteLine("Get details about order? (Y/n)");
        string input = Console.ReadLine().Trim();

        switch (input)
        {
            case "Y":
                Console.WriteLine("Enter the order ID:");
                int id = int.Parse(Console.ReadLine().Trim());
                Query.OrderDetails(id);
                Query.ItemsInOrder(id);
                break;
            case "n":
                break;
            default:
                Console.WriteLine("Not a valid input!");
                break;
        }
    }
    
    
}