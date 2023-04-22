namespace Uhhh;

public class Admin
{
    public void AdminMenu()
    {
        string menu = @"Select your choice:
1) Add a new product to the warehouse
2) Delete a product in the warehouse
3) Update the quantity of a product in the warehouse
4) Execute a query of choice
0) Exit
";
        bool run = true;
        while (run)
        {
            int input = int.Parse(Console.ReadLine().Trim());
            switch (input)
            {
                case 1:
                    AddNewProduct();
                    break;
                case 2:
                    DeleteProduct();
                    break;
                case 3:
                    UpdateProductQuantity();
                break;
                case 4:
                    ExecuteCustomQuery();
                break;
                case 0:
                    run = false;
                    break;
                default:
                    Console.WriteLine("Not a valid input!");
                    break;
            }
        }
    }

    private void AddNewProduct()
    {
        Console.WriteLine("Enter the name of the new product:");
        string name = Console.ReadLine().Trim();
        Console.WriteLine("Enter the manufacturer of the product:");
        string manufacturer = Console.ReadLine().Trim();
        Console.WriteLine("Enter the price of the new product:");
        int price = int.Parse(Console.ReadLine().Trim());
        Console.WriteLine("Enter the quantity of the product to be added");
        int quantity = int.Parse(Console.ReadLine().Trim());
        Console.WriteLine("Provide a new product ID for the new product:");
        int prodId = int.Parse(Console.ReadLine().Trim());
        
        Query.AddToWarehouse(name, manufacturer, price, quantity, prodId);
        Console.WriteLine("Added!");
    }

    private void DeleteProduct()
    {
        Console.WriteLine("Enter the product ID of the product to delete it");
        int prodId = int.Parse(Console.ReadLine().Trim());
        
        Query.DeleteProduct(prodId);
        Console.WriteLine("Product deleted!");
    }

    private void UpdateProductQuantity()
    {
        Console.WriteLine("Enter the product ID:");
        int id = int.Parse(Console.ReadLine().Trim());
        Console.WriteLine("Enter the new quantity:");
        int qty = int.Parse(Console.ReadLine().Trim());
        
        Query.UpdateProductQuantity(id, qty);
        Console.WriteLine("Quantity updated!");
    }

    private void ExecuteCustomQuery()
    {
        Console.WriteLine("With great power comes great responsibility! Enter the query:");
        string query = Console.ReadLine().Trim();
        
        Query.CustomQuery(query);
    }
    
}