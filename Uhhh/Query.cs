using MySql.Data.MySqlClient;
using System.Data;

namespace Uhhh;

internal static class Query
{
    private static MySqlCommand? _command;
    private static MySqlDataReader? _reader;

    // statically called queries
    private static void Preprocessing()
    {
        _command = new MySqlCommand();
        _command.Connection = Pharma.Conn;
        _command!.CommandType = CommandType.Text;
    }

    public static string GetPassword(string username)
    {
        try
        {
            Preprocessing();
            _command!.CommandText = $"SELECT Password FROM pharmacy_prod.Customers WHERE Name = '{username}'";
            _reader = _command.ExecuteReader();
            var pwd = "0";
            while (_reader.Read())
            {
                pwd = _reader[0].ToString()!;
            }
            _reader.Close();
            return pwd;
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            return "";
        }
    }

    public static int GetCustomerId(string username)
    {
        try
        {
            Preprocessing();
            _command!.CommandText = $"SELECT CustomerID FROM pharmacy_prod.Customers WHERE Name = '{username}'";
            var pwd = "0";
            _reader = _command.ExecuteReader();
            while (_reader.Read())
            {
                pwd = _reader[0].ToString()!;
            }
            _reader.Close();
            return int.Parse(pwd);
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            throw;
        }

    }
    
    public static void ViewProducts(int threshold, string like)
    {
        try
        {
            Preprocessing();
            _command!.CommandText = $"SELECT * FROM pharmacy_prod.Products WHERE Price < {threshold} AND Name LIKE '%{like}%'";

            _reader = _command.ExecuteReader();
            Console.WriteLine("Name            Manufacturer                      Price   Quantity Available");
            while (_reader.Read())
            {
                // var pid = _reader[0].ToString()!.PadRight("ProductID    ".Length);
                var name = _reader[1].ToString()!.PadRight(16);
                var manufacturer = _reader[2].ToString()!.PadRight("Schumm, Pfannerstill and Lueilwitz".Length);
                var price = _reader[3].ToString()!.PadRight("Price   ".Length);
        
        
                Console.WriteLine(name + manufacturer + price + _reader[4]);
            }
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    private static int GetProductId(string name)
    {
        try
        {
            Preprocessing();
            _command!.CommandText = $"SELECT ProductID FROM pharmacy_prod.Products WHERE Name = '{name}'";
            var pwd = "0";
            _reader = _command.ExecuteReader();
            while (_reader.Read())
            {
                pwd = _reader[0].ToString()!;
            }
            _reader.Close();
            return int.Parse(pwd);
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            throw;

        }

    }

    public static void AddToCart(string name, int quantity, int customerId)
    {
        try
        {
            var pid = GetProductId(name);
            Preprocessing();
            _command!.CommandText =
                $"INSERT INTO pharmacy_prod.Cart (CustomerID, ProductID, Quantity) VALUES ({customerId}, {pid}, {quantity})";
            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    public static void InspectCart(int customerId)
    {
        try
        {
            Preprocessing();
            _command!.CommandText =
                $"SELECT p.Name, p.Price, c.Quantity FROM pharmacy_prod.Cart c JOIN pharmacy_prod.Products p on c.ProductID = p.ProductID WHERE CustomerID = {customerId}";

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
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    public static int CartCost(int customerId)
    {
        try
        {
            Preprocessing();
            _command!.CommandText =
                $"SELECT SUM(c.Quantity * p.Price) FROM pharmacy_prod.Cart c JOIN pharmacy_prod.Products p on c.ProductID = p.ProductID WHERE CustomerID = {customerId}";
        
            var pwd = "0";
            _reader = _command.ExecuteReader();
            while (_reader.Read())
            {
                pwd = _reader[0].ToString()!;
            }
            _reader.Close();
            return int.Parse(pwd);
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            throw;

        }
    }

    public static bool CartSanity(int customerId)
    {

        try
        {
            Preprocessing();
            _command!.CommandText =
                $"SELECT COUNT(*) FROM pharmacy_prod.Cart c JOIN pharmacy_prod.Products p on c.ProductID = p.ProductID WHERE c.Quantity > p.Quantity AND CustomerID = {customerId}";
        
            var pwd = "0";
            _reader = _command.ExecuteReader();
            while (_reader.Read())
            {
                pwd = _reader[0].ToString()!;
            }
            _reader.Close();

            return int.Parse(pwd) == 0;
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            return false;

        }

    }

    public static void ClearCart(int customerId, string mode)
    {
        try
        {
            AddToOrders(customerId, mode);
            Preprocessing();
            _command!.CommandText = $"SELECT ProductID, Quantity FROM pharmacy_prod.Cart WHERE CustomerID = {customerId}";
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
                AddToOrderedItems(GetNewOrderId(), data[(2 * i) + 1], data[2 * i]);
                SubtractProductQty(data[2*i + 1], data[2*i]);
            }
            Preprocessing();
            ClearCartFromWarehouse(customerId);
            Preprocessing();
            _command.CommandText = $"DELETE FROM pharmacy_prod.Cart WHERE CustomerID = {customerId}";

            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    private static void AddToOrders(int customerId, string mode)
    {

        try
        {
            Preprocessing();
            var newOrderId = GetNewOrderId() + 1;
            Preprocessing();
            var cartCost = CartCost(customerId);
            Preprocessing();
            var guy = AssignDelivery();
            Preprocessing();
            _command!.CommandText =
                "INSERT INTO pharmacy_prod.Orders (OrderID, OrderDate, Status, Cost, PaymentType, CorrespondentID, CustomerID) VALUES" +
                $" ({newOrderId}, NOW(), 'Scheduled',{cartCost}, '{mode}', {guy}, {customerId})";
            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            throw;

        }
    }

    private static void ClearCartFromWarehouse(int customerId)
    {
        try
        {
            Preprocessing();
            _command!.CommandText = $"SELECT ProductID, Quantity FROM pharmacy_prod.Cart WHERE CustomerID = {customerId}";
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
                _command.CommandText = $"CALL pharmacy_prod.DeleteProducts({data[2*i + 1]}, {data[2*i]})";
                _reader = _command.ExecuteReader();
                _reader.Close();
            }
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    private static void AddToOrderedItems(int orderId, int productId, int qty)
    {
        try
        {
            Preprocessing();
            
            _command!.CommandText = $"CALL pharmacy_prod.AddToOrderedItems({qty}, {orderId}, {productId})";

            Console.WriteLine(_command.CommandText);
            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    public static void GetCustomerOrders(int customerId)
    {
        try
        {
            Preprocessing();
            _command!.CommandText = $"SELECT * FROM pharmacy_prod.Orders WHERE CustomerID = {customerId}";

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
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    public static void ItemsInOrder(int orderId)
    {

        try
        {
            Preprocessing();
            _command!.CommandText =
                $"SELECT TEMP.ProductID, TEMP.quantity, Name, Price " +
                $"FROM (SELECT COUNT(O.UnitID) AS quantity, ProductID FROM (pharmacy_prod.OrderedItems O JOIN pharmacy_prod.Warehouse W on O.UnitID = W.UnitID) WHERE O.OrderID =  {orderId} GROUP BY ProductID) as TEMP JOIN pharmacy_prod.Products p on TEMP.ProductID = p.ProductID";
        
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
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }
    
    public static void OrderDetails(int orderId) 
    {

        try
        {
            Preprocessing();
            _command!.CommandText = $"SELECT * FROM pharmacy_prod.Orders WHERE OrderID = {orderId}";

            _reader = _command.ExecuteReader();
            while (_reader.Read())
            {
                var details =
                    $"Order Date: {_reader[1]}\nStatus: {_reader[2]}\nCost: {_reader[3]}\nPayment Type: {_reader[4]}\nCorrespondent ID: {_reader[5]}";
            
                Console.WriteLine(details);
            }
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }

    }

    public static void SubtractProductQty(int prodId, int cut)
    {
        Preprocessing();
        _command!.CommandText = $"UPDATE pharmacy_prod.Products SET Quantity = Quantity - {cut} WHERE ProductID = {prodId}";
        _reader = _command.ExecuteReader();
        _reader.Close();

    }
    
    // time for admin privileges
    public static void CustomQuery(string query)
    {
        try
        {
            Preprocessing();
            _command!.CommandText = query;

            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    public static void AddToWarehouse(string name, string manuf, int price, int quantity, int prodId)
    {
        try
        {
            Preprocessing();
            _command!.CommandText =
                $"INSERT INTO pharmacy_prod.Products (ProductID, Name, Manufacturer, Price, Quantity) VALUES ({prodId},'{name}','{manuf}', {price}, {quantity})";

            _reader = _command.ExecuteReader();
            _reader.Close();
        
            LoadNewProducts(quantity, prodId);
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }

    }

    public static void DeleteProduct(int prodId)
    {
        try
        {
            Preprocessing();
            DeleteProducts(GetProductQuantity(prodId), prodId);
            _command!.CommandText = $"DELETE FROM pharmacy_prod.Products WHERE ProductID = {prodId}";

            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    public static void UpdateProductQuantity(int id, int qty)
    {
        try
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
            _command!.CommandText = $"UPDATE pharmacy_prod.Products SET Quantity = {qty} WHERE ProductID = {id}";

            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    public static void LoadNewProducts(int qty, int prodId)
    {

        try
        {
            Preprocessing();
            _command!.CommandText = $"CALL pharmacy_prod.LoadNewProducts({qty}, {prodId})";
        
            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }
    }

    private static void DeleteProducts(int qty, int prodId)
    {

        try
        {
            Preprocessing();
            _command!.CommandText = $"CALL pharmacy_prod.DeleteProducts({qty}, {prodId})";

            _reader = _command.ExecuteReader();
            _reader.Close();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            
        }

    }

    public static int GetProductQuantity(int prodId)
    {

        try
        {
            Preprocessing();
            _command!.CommandText = $"SELECT Quantity FROM pharmacy_prod.Products WHERE ProductID = {prodId}";
            _reader = _command.ExecuteReader();
            var pwd = "0";
            while (_reader.Read())
            {
                pwd = _reader[0].ToString()!;
            }
            _reader.Close();
            return int.Parse(pwd);
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            throw;
        }

    }

    private static int GetNewOrderId()
    {

        try
        {
            Preprocessing();
            _command!.CommandText = "SELECT COUNT(OrderID) FROM pharmacy_prod.Orders";
            _reader = _command.ExecuteReader();
            var n = "0";
            while (_reader.Read())
            {
                n = _reader[0].ToString()!.Trim();
            }
            _reader.Close();
            return int.Parse(n);
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            throw;

        }

    }

    private static int AssignDelivery()
    {

        try
        {
            Preprocessing();
            _command!.CommandText = "SELECT CorrespondentID FROM pharmacy_prod.Orders WHERE Status = '0' LIMIT 1";

            _reader = _command.ExecuteReader();
            int bruh;
            while (_reader.Read())
            {
                bruh = int.Parse(_reader[0].ToString()!.Trim());
            }
            _reader.Close();
            return 3;
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Server error!");
            throw;
        }
    }
}
