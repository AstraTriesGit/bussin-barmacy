using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;
using Uhhh;

public class Query
{
    private static MySqlCommand command;
    private static MySqlDataReader reader;

    // objects are use-once
    private static void Preprocessing()
    {
        command = new MySqlCommand();
        command.Connection = Pharma.conn;
        command.CommandType = CommandType.Text;
    }

    public static string GetPassword(string username)
    {
        Preprocessing();
        command.CommandText = "SELECT Password FROM pharmacy.Customers WHERE Name = '" + username + "'";
        reader = command.ExecuteReader();
        string pwd = "0";
        while (reader.Read())
        {
            pwd = reader[0].ToString()!;
        }
        reader.Close();
        return pwd;
    }

    public static int GetCustomerID(string username)
    {
        Preprocessing();
        command.CommandText = "SELECT Customer_ID FROM pharmacy.Customers WHERE Name = '" + username + "'";
        string pwd = "0";
        reader = command.ExecuteReader();
        while (reader.Read())
        {
            pwd = reader[0].ToString()!;
        }
        reader.Close();
        return int.Parse(pwd);
    }
    
    public static void ViewProducts(int threshold, string like)
    {
        Preprocessing();
        command.CommandText = "SELECT * FROM pharmacy.Products WHERE Price < " + threshold + " AND Name LIKE '" + like + "'";

        reader = command.ExecuteReader();
        Console.WriteLine("ProductID    Name            Manufacturer                      Price   Quantity Available");
        while (reader.Read())
        {
            string pid = reader[0].ToString()!.PadRight("ProductID    ".Length);
            string name = reader[1].ToString()!.PadRight(16);
            string manufacturer = reader[2].ToString()!.PadRight("Schumm, Pfannerstill and Lueilwitz".Length);
            string price = reader[3].ToString()!.PadRight("Price   ".Length);
        
        
            Console.WriteLine(pid + name + manufacturer + price + reader[4]);
        }
        reader.Close();
    }

    public static int GetProductID(string name)
    {
        Preprocessing();
        command.CommandText = "SELECT ProductID FROM pharmacy.Products WHERE Name = '" + name + "'";
        string pwd = "0";
        reader = command.ExecuteReader();
        while (reader.Read())
        {
            pwd = reader[0].ToString()!;
        }
        reader.Close();
        return int.Parse(pwd);
    }

    public static void AddToCart(string name, int quantity, int customerId)
    {
        Preprocessing();
        command.CommandText = "INSERT INTO pharmacy.Cart (CustomerID, ProductID, Quantity) VALUES (" +
                              customerId + ", " +
                              GetProductID(name) + ", " +
                              quantity + ")";
        reader = command.ExecuteReader();
        reader.Close();
    }

    public static void InspectCart(int customerId)
    {
        Preprocessing();
        command.CommandText = "SELECT p.Name, p.Price, c.Quantity FROM pharmacy.Cart c JOIN pharmacy.Products p on c.ProductID = p.ProductID " +
                              "WHERE CustomerID = " + customerId;

        reader = command.ExecuteReader();
        Console.WriteLine("Name            Price   Quantity");
        while (reader.Read())
        {
            string name = reader[0].ToString()!.PadRight("Name            ".Length);
            string price = reader[1].ToString()!.PadRight("Price   ".Length);
            string quantity = reader[2].ToString()!;
            
            Console.WriteLine(name + price + quantity);
        }
        reader.Close();
    }

    public static int CartCost(int customerId)
    {
        Preprocessing();
        command.CommandText = "SELECT SUM(c.Quantity * p.Price) FROM pharmacy.Cart c JOIN pharmacy.Products p on c.ProductID = p.ProductID" +
                              " WHERE CustomerID = " + customerId;
        
        string pwd = "0";
        reader = command.ExecuteReader();
        while (reader.Read())
        {
            pwd = reader[0].ToString()!;
        }
        reader.Close();
        return int.Parse(pwd);
    }

    public static bool CartSanity(int customerId)
    {
        Preprocessing();
        command.CommandText =
            "SELECT COUNT(*) FROM pharmacy.Cart c JOIN pharmacy.Products p on c.ProductID = p.ProductID " +
            "WHERE c.Quantity > p.Quantity AND CustomerID = " + customerId;
        
        string pwd = "0";
        reader = command.ExecuteReader();
        while (reader.Read())
        {
            pwd = reader[0].ToString()!;
        }
        reader.Close();

        return int.Parse(pwd) == 0;
    }

    public static void ClearCart(int customerId, string mode)
    {
        AddToOrders(customerId, mode);
        Preprocessing();
        command.CommandText = "SELECT ProductID, Quantity FROM pharmacy.Cart WHERE CustomerID = " + customerId;
        reader = command.ExecuteReader();
        List<int> data = new List<int>();
        while (reader.Read())
        {
            // prod id
            data.Add(int.Parse(reader[0].ToString().Trim()));
            // qty
            data.Add(int.Parse(reader[1].ToString().Trim()));
        }
        reader.Close();
        for (int i = 0; i < data.Count/2; i++)
        {
            AddToOrderedItems(GetNewOrderID(), data[0], data[1]);
        }
        Preprocessing();
        ClearCartFromWarehouse(customerId);
        Preprocessing();
        command.CommandText = "DELETE FROM pharmacy.Cart WHERE CustomerID = " + customerId;

        reader = command.ExecuteReader();
        reader.Close();
    }

    public static void AddToOrders(int customerId, string mode)
    {
        Preprocessing();
        int newOrderId = GetNewOrderID() + 1;
        command.CommandText = "INSERT INTO pharmacy.Orders (OrderID, OrderDate, Status, Cost, PaymentType, CorrespondentID, CustomerID) VALUES " +
                              "(" +
                              newOrderId + ", " +
                              "NOW(), " +
                              "'Scheduled'," +
                              CartCost(customerId) + ", " +
                              mode + ", " +
                              AssignDelivery() + ", " +
                              customerId + ", " +
                              ")";
        reader = command.ExecuteReader();
        reader.Close();
    }

    public static void ClearCartFromWarehouse(int customerId)
    {
        Preprocessing();
        command.CommandText = "SELECT ProductID, Quantity FROM pharmacy.Cart WHERE CustomerID = " + customerId;
        reader = command.ExecuteReader();
        List<int> data = new List<int>();
        while (reader.Read())
        {
            int prodId = int.Parse(reader[0].ToString()!.Trim());
            int qty = int.Parse(reader[1].ToString()!.Trim());
            data.Add(prodId);
            data.Add(qty);
        }
        reader.Close();
        
        for (int i = 0; i < data.Count/2; i++)
        {
            Preprocessing();
            command.CommandText = "CALL pharmacy.DeleteProducts(" + data[i + 1] + ", " + data[i] + ")";

            reader = command.ExecuteReader();
            reader.Close();
        }
        
    }

    public static void AddToOrderedItems(int orderId, int productId, int qty)
    {
        Preprocessing();
        command.CommandText = "INSERT INTO pharmacy.OrderedItems (" +
                              "SELECT UnitID AS Unit_ID," + orderId +" AS OrderId " +
                              "FROM pharmacy.Warehouse " +
                              "WHERE ProductID = " + productId +
                              " LIMIT " + qty +
                              ")";

        reader = command.ExecuteReader();
        reader.Close();
    }

    public static void GetCustomerOrders(int customerId)
    {
        Preprocessing();
        command.CommandText = "SELECT * FROM pharmacy.Orders WHERE CustomerID = " + customerId;

        reader = command.ExecuteReader();
        Console.WriteLine("OrderID    OrderDate           Status            Cost            Payment Type      Correspondent ID");
        while (reader.Read())
        {
            string oid = reader[0].ToString()!.PadRight("OrderID    ".Length);
            string orderDate = reader[1].ToString()!.PadRight("OrderDate           ".Length);
            string status = reader[2].ToString()!.PadRight("Status            ".Length);
            string cost = reader[3].ToString()!.PadRight("Cost            ".Length);
            string paymentType = reader[4].ToString()!.PadRight("Payment Type      ".Length);
            string correspondentId = reader[5].ToString()!;
        
        
            Console.WriteLine(oid, orderDate, status, cost, paymentType, correspondentId);
        }
        reader.Close();
    }

    public static void ItemsInOrder(int orderId)
    {
        Preprocessing();
        command.CommandText = "SELECT TEMP.ProductID, TEMP.quantity, Name, Price " +
                              "FROM (SELECT COUNT(O.Unit_ID) AS quantity, ProductID " +
                              "FROM (pharmacy.OrderedItems O JOIN pharmacy.Warehouse W on O.Unit_ID = W.UnitID) " +
                              "WHERE O.OrderID =  " + orderId +
                              " GROUP BY ProductID) as TEMP " +
                              "JOIN pharmacy.Products p on TEMP.ProductID = p.ProductID";
        
        reader = command.ExecuteReader();
        Console.WriteLine("Name            Price            Quantity");
        while (reader.Read())
        {
            string name = reader[2].ToString()!.PadRight("Name            ".Length);
            string price = reader[3].ToString()!.PadRight("Price            ".Length);
            string quantity = reader[1].ToString()!.PadRight("Status            ".Length);

            Console.WriteLine(name, price, quantity);
        }
        reader.Close();
    }
    
    public static void OrderDetails(int orderId) 
    {
        Preprocessing();
        command.CommandText = "SELECT * FROM pharmacy.Orders WHERE OrderID = " + orderId;

        reader = command.ExecuteReader();
        while (reader.Read())
        {
            string details = "Order Date: " + reader[1] +
                             "\nStatus: " + reader[2] +
                             "\nCost: " + reader[3] +
                             "\nPayment Type: " + reader[4] +
                             "\nCorrespondent ID: " + reader[5];
            
            Console.WriteLine(details);
        }
        reader.Close();
    }
    
    // time for admin privileges
    public static void CustomQuery(string query)
    {
        Preprocessing();
        command.CommandText = query;

        reader = command.ExecuteReader();
        reader.Close();
    }

    public static void AddToWarehouse(string name, string manuf, int price, int quantity, int prodId)
    {
        Preprocessing();
        command.CommandText = "INSERT INTO pharmacy.Products (ProductID, Name, Manufacturer, Price, Quantity) VALUES (" 
                              + prodId + 
                              ",'" + name +
                              "','" + manuf +
                              "', " + price +
                              ", " + quantity +
                              ")";

        reader = command.ExecuteReader();
        reader.Close();
        
        LoadNewProducts(quantity, prodId);
    }

    public static void DeleteProduct(int prodId)
    {
        Preprocessing();
        DeleteProducts(GetProductQuantity(prodId), prodId);
        command.CommandText = "DELETE FROM pharmacy.Products WHERE ProductID = " + prodId;

        reader = command.ExecuteReader();
        reader.Close();
    }

    public static void UpdateProductQuantity(int id, int qty)
    {
        Preprocessing();
        if (qty > GetProductQuantity(id))
        {
            LoadNewProducts(qty - GetProductQuantity(id), id);
        }
        else if(qty < GetProductQuantity(id))
        {
            DeleteProducts(GetProductQuantity(id) - qty, id);
        }
        else
        {
            return;
        }
        Preprocessing();
        command.CommandText = "UPDATE pharmacy.Products " +
                              "SET Quantity = " + qty +
                              " WHERE ProductID = " + id;

        reader = command.ExecuteReader();
        reader.Close();
    }

    public static void LoadNewProducts(int qty, int prodId)
    {
        Preprocessing();
        command.CommandText = "CALL pharmacy.LoadNewProducts("
                              + qty +
                              ", " + prodId +
                              ")";
        
        reader = command.ExecuteReader();
        reader.Close();
    }

    public static void DeleteProducts(int qty, int prodId)
    {
        Preprocessing();
        command.CommandText = "CALL pharmacy.DeleteProducts("
                              + qty +
                              ", " + prodId +
                              ")";

        reader = command.ExecuteReader();
        reader.Close();
    }

    public static int GetProductQuantity(int prodId)
    {
        Preprocessing();
        command.CommandText = "SELECT Quantity FROM pharmacy.Products WHERE ProductID = " + prodId;
        reader = command.ExecuteReader();
        string pwd = "0";
        while (reader.Read())
        {
            pwd = reader[0].ToString()!;
        }
        reader.Close();
        return int.Parse(pwd);
    }

    public static int GetNewOrderID()
    {
        Preprocessing();
        command.CommandText = "SELECT MAX(OrderID) FROM pharmacy.Orders";
        reader = command.ExecuteReader();
        string n = "0";
        while (reader.Read())
        {
            n = reader[0].ToString().Trim();
        }
        reader.Close();
        return int.Parse(n);
    }

    public static int AssignDelivery()
    {
        Preprocessing();
        command.CommandText = "SELECT CorrespondentID FROM pharmacy.Orders WHERE Status = 'Delivered'";

        reader = command.ExecuteReader();
        List<int> ids = new List<int>();
        while (reader.Read())
        {
            ids.Add(int.Parse(reader[0].ToString().Trim()));
        }
        reader.Close();

        var rng = new Random();
        int guy = rng.Next(ids.Count);  
        
        return guy;
    }
}
