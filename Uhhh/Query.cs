using MySql.Data.MySqlClient;
using System.Data;
using Uhhh;

internal static class Query
{
    private static MySqlCommand? _command;
    private static MySqlDataReader? _reader;

    // objects are use-once
    private static void Preprocessing()
    {
        _command = new MySqlCommand();
        _command.Connection = Pharma.Conn;
        _command!.CommandType = CommandType.Text;
    }

    public static string GetPassword(string username)
    {
        Preprocessing();
        _command!.CommandText = $"SELECT Password FROM pharmacy.Customers WHERE Name = '{username}'";
        _reader = _command.ExecuteReader();
        var pwd = "0";
        while (_reader.Read())
        {
            pwd = _reader[0].ToString()!;
        }
        _reader.Close();
        return pwd;
    }

    public static int GetCustomerId(string username)
    {
        Preprocessing();
        _command!.CommandText = $"SELECT Customer_ID FROM pharmacy.Customers WHERE Name = '{username}'";
        var pwd = "0";
        _reader = _command.ExecuteReader();
        while (_reader.Read())
        {
            pwd = _reader[0].ToString()!;
        }
        _reader.Close();
        return int.Parse(pwd);
    }
    
    public static void ViewProducts(int threshold, string like)
    {
        Preprocessing();
        _command!.CommandText = $"SELECT * FROM pharmacy.Products WHERE Price < {threshold} AND Name LIKE '{like}'";

        _reader = _command.ExecuteReader();
        Console.WriteLine("ProductID    Name            Manufacturer                      Price   Quantity Available");
        while (_reader.Read())
        {
            var pid = _reader[0].ToString()!.PadRight("ProductID    ".Length);
            var name = _reader[1].ToString()!.PadRight(16);
            var manufacturer = _reader[2].ToString()!.PadRight("Schumm, Pfannerstill and Lueilwitz".Length);
            var price = _reader[3].ToString()!.PadRight("Price   ".Length);
        
        
            Console.WriteLine(pid + name + manufacturer + price + _reader[4]);
        }
        _reader.Close();
    }

    private static int GetProductId(string name)
    {
        Preprocessing();
        _command!.CommandText = $"SELECT ProductID FROM pharmacy.Products WHERE Name = '{name}'";
        var pwd = "0";
        _reader = _command.ExecuteReader();
        while (_reader.Read())
        {
            pwd = _reader[0].ToString()!;
        }
        _reader.Close();
        return int.Parse(pwd);
    }

    public static void AddToCart(string name, int quantity, int customerId)
    {
        Preprocessing();
        _command!.CommandText =
            $"INSERT INTO pharmacy.Cart (CustomerID, ProductID, Quantity) VALUES ({customerId}, {GetProductId(name)}, {quantity})";
        _reader = _command.ExecuteReader();
        _reader.Close();
    }

    public static void InspectCart(int customerId)
    {
        Preprocessing();
        _command!.CommandText =
            $"SELECT p.Name, p.Price, c.Quantity FROM pharmacy.Cart c JOIN pharmacy.Products p on c.ProductID = p.ProductID WHERE CustomerID = {customerId}";

        _reader = _command.ExecuteReader();
        Console.WriteLine("Name            Price   Quantity");
        while (_reader.Read())
        {
            var name = _reader[0].ToString()!.PadRight("Name            ".Length);
            var price = _reader[1].ToString()!.PadRight("Price   ".Length);
            var quantity = _reader[2].ToString()!;
            
            Console.WriteLine(name + price + quantity);
        }
        _reader.Close();
    }

    public static int CartCost(int customerId)
    {
        Preprocessing();
        _command!.CommandText =
            $"SELECT SUM(c.Quantity * p.Price) FROM pharmacy.Cart c JOIN pharmacy.Products p on c.ProductID = p.ProductID WHERE CustomerID = {customerId}";
        
        var pwd = "0";
        _reader = _command.ExecuteReader();
        while (_reader.Read())
        {
            pwd = _reader[0].ToString()!;
        }
        _reader.Close();
        return int.Parse(pwd);
    }

    public static bool CartSanity(int customerId)
    {
        Preprocessing();
        _command!.CommandText =
            $"SELECT COUNT(*) FROM pharmacy.Cart c JOIN pharmacy.Products p on c.ProductID = p.ProductID WHERE c.Quantity > p.Quantity AND CustomerID = {customerId}";
        
        var pwd = "0";
        _reader = _command.ExecuteReader();
        while (_reader.Read())
        {
            pwd = _reader[0].ToString()!;
        }
        _reader.Close();

        return int.Parse(pwd) == 0;
    }

    public static void ClearCart(int customerId, string mode)
    {
        AddToOrders(customerId, mode);
        Preprocessing();
        _command!.CommandText = $"SELECT ProductID, Quantity FROM pharmacy.Cart WHERE CustomerID = {customerId}";
        _reader = _command.ExecuteReader();
        var data = new List<int>();
        while (_reader.Read())
        {
            // prod id
            data.Add(int.Parse(_reader[0].ToString()!.Trim()));
            // qty
            data.Add(int.Parse(_reader[1].ToString()!.Trim()));
        }
        _reader.Close();
        for (var i = 0; i < data.Count/2; i++)
        {
            AddToOrderedItems(GetNewOrderId(), data[0], data[1]);
        }
        Preprocessing();
        ClearCartFromWarehouse(customerId);
        Preprocessing();
        _command.CommandText = $"DELETE FROM pharmacy.Cart WHERE CustomerID = {customerId}";

        _reader = _command.ExecuteReader();
        _reader.Close();
    }

    private static void AddToOrders(int customerId, string mode)
    {
        Preprocessing();
        var newOrderId = GetNewOrderId() + 1;
        _command!.CommandText =
            $"INSERT INTO pharmacy.Orders (OrderID, OrderDate, Status, Cost, PaymentType, CorrespondentID, CustomerID) VALUES ({newOrderId}, NOW(), 'Scheduled',{CartCost(customerId)}, {mode}, {AssignDelivery()}, {customerId}, )";
        _reader = _command.ExecuteReader();
        _reader.Close();
    }

    private static void ClearCartFromWarehouse(int customerId)
    {
        Preprocessing();
        _command!.CommandText = $"SELECT ProductID, Quantity FROM pharmacy.Cart WHERE CustomerID = {customerId}";
        _reader = _command.ExecuteReader();
        var data = new List<int>();
        while (_reader.Read())
        {
            var prodId = int.Parse(_reader[0].ToString()!.Trim());
            var qty = int.Parse(_reader[1].ToString()!.Trim());
            data.Add(prodId);
            data.Add(qty);
        }
        _reader.Close();
        
        for (var i = 0; i < data.Count/2; i++)
        {
            Preprocessing();
            _command.CommandText = $"CALL pharmacy.DeleteProducts({data[i + 1]}, {data[i]})";

            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        
    }

    private static void AddToOrderedItems(int orderId, int productId, int qty)
    {
        Preprocessing();
        _command!.CommandText =
            $"INSERT INTO pharmacy.OrderedItems (SELECT UnitID AS Unit_ID,{orderId} AS OrderId FROM pharmacy.Warehouse WHERE ProductID = {productId} LIMIT {qty})";

        _reader = _command.ExecuteReader();
        _reader.Close();
    }

    public static void GetCustomerOrders(int customerId)
    {
        Preprocessing();
        _command!.CommandText = $"SELECT * FROM pharmacy.Orders WHERE CustomerID = {customerId}";

        _reader = _command.ExecuteReader();
        Console.WriteLine("OrderID    OrderDate           Status            Cost            Payment Type      Correspondent ID");
        while (_reader.Read())
        {
            var oid = _reader[0].ToString()!.PadRight("OrderID    ".Length);
            var orderDate = _reader[1].ToString()!.PadRight("OrderDate           ".Length);
            var status = _reader[2].ToString()!.PadRight("Status            ".Length);
            var cost = _reader[3].ToString()!.PadRight("Cost            ".Length);
            var paymentType = _reader[4].ToString()!.PadRight("Payment Type      ".Length);
            var correspondentId = _reader[5].ToString()!;
        
        
            Console.WriteLine(oid, orderDate, status, cost, paymentType, correspondentId);
        }
        _reader.Close();
    }

    public static void ItemsInOrder(int orderId)
    {
        Preprocessing();
        _command!.CommandText =
            $"SELECT TEMP.ProductID, TEMP.quantity, Name, Price FROM (SELECT COUNT(O.Unit_ID) AS quantity, ProductID FROM (pharmacy.OrderedItems O JOIN pharmacy.Warehouse W on O.Unit_ID = W.UnitID) WHERE O.OrderID =  {orderId} GROUP BY ProductID) as TEMP JOIN pharmacy.Products p on TEMP.ProductID = p.ProductID";
        
        _reader = _command.ExecuteReader();
        Console.WriteLine("Name            Price            Quantity");
        while (_reader.Read())
        {
            var name = _reader[2].ToString()!.PadRight("Name            ".Length);
            var price = _reader[3].ToString()!.PadRight("Price            ".Length);
            var quantity = _reader[1].ToString()!.PadRight("Status            ".Length);

            Console.WriteLine(name, price, quantity);
        }
        _reader.Close();
    }
    
    public static void OrderDetails(int orderId) 
    {
        Preprocessing();
        _command!.CommandText = $"SELECT * FROM pharmacy.Orders WHERE OrderID = {orderId}";

        _reader = _command.ExecuteReader();
        while (_reader.Read())
        {
            var details =
                $"Order Date: {_reader[1]}\nStatus: {_reader[2]}\nCost: {_reader[3]}\nPayment Type: {_reader[4]}\nCorrespondent ID: {_reader[5]}";
            
            Console.WriteLine(details);
        }
        _reader.Close();
    }
    
    // time for admin privileges
    public static void CustomQuery(string query)
    {
        Preprocessing();
        _command!.CommandText = query;

        _reader = _command.ExecuteReader();
        _reader.Close();
    }

    public static void AddToWarehouse(string name, string manuf, int price, int quantity, int prodId)
    {
        Preprocessing();
        _command!.CommandText =
            $"INSERT INTO pharmacy.Products (ProductID, Name, Manufacturer, Price, Quantity) VALUES ({prodId},'{name}','{manuf}', {price}, {quantity})";

        _reader = _command.ExecuteReader();
        _reader.Close();
        
        LoadNewProducts(quantity, prodId);
    }

    public static void DeleteProduct(int prodId)
    {
        Preprocessing();
        DeleteProducts(GetProductQuantity(prodId), prodId);
        _command!.CommandText = $"DELETE FROM pharmacy.Products WHERE ProductID = {prodId}";

        _reader = _command.ExecuteReader();
        _reader.Close();
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
        _command!.CommandText = $"UPDATE pharmacy.Products SET Quantity = {qty} WHERE ProductID = {id}";

        _reader = _command.ExecuteReader();
        _reader.Close();
    }

    public static void LoadNewProducts(int qty, int prodId)
    {
        Preprocessing();
        _command!.CommandText = $"CALL pharmacy.LoadNewProducts({qty}, {prodId})";
        
        _reader = _command.ExecuteReader();
        _reader.Close();
    }

    private static void DeleteProducts(int qty, int prodId)
    {
        Preprocessing();
        _command!.CommandText = $"CALL pharmacy.DeleteProducts({qty}, {prodId})";

        _reader = _command.ExecuteReader();
        _reader.Close();
    }

    public static int GetProductQuantity(int prodId)
    {
        Preprocessing();
        _command!.CommandText = $"SELECT Quantity FROM pharmacy.Products WHERE ProductID = {prodId}";
        _reader = _command.ExecuteReader();
        var pwd = "0";
        while (_reader.Read())
        {
            pwd = _reader[0].ToString()!;
        }
        _reader.Close();
        return int.Parse(pwd);
    }

    private static int GetNewOrderId()
    {
        Preprocessing();
        _command!.CommandText = "SELECT MAX(OrderID) FROM pharmacy.Orders";
        _reader = _command.ExecuteReader();
        var n = "0";
        while (_reader.Read())
        {
            n = _reader[0].ToString()!.Trim();
        }
        _reader.Close();
        return int.Parse(n);
    }

    private static int AssignDelivery()
    {
        Preprocessing();
        _command!.CommandText = "SELECT CorrespondentID FROM pharmacy.Orders WHERE Status = 'Delivered'";

        _reader = _command.ExecuteReader();
        var ids = new List<int>();
        while (_reader.Read())
        {
            ids.Add(int.Parse(_reader[0].ToString()!.Trim()));
        }
        _reader.Close();

        var rng = new Random();
        var guy = rng.Next(ids.Count);  
        
        return guy;
    }
}
