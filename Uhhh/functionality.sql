USE pharmacy;

# preprocessing
-- password(str username)
SELECT Customer_ID FROM Customers WHERE Name = 'Cullen Orn';


# customer functionality
-- addtocart(int name_to_id(), int quantity)
INSERT INTO Cart (CustomerID, ProductID, Quantity) VALUES (97, 201, 69);

-- name_to_id(str product_name)
SELECT ProductID FROM Products WHERE Name = 'Herwoman';

-- console rip
SELECT * FRom information_schema.TABLES WHERE TABLE_SCHEMA = 'pharmacy';

SELECT * From Cart;
SELECT * FROM Customers;
SELECT * From Products;
SELECT * FROM Orders;
SELECT * FROM OrderedItems;
SELECT * FROM Warehouse;

SELECT * FROM pharmacy.Products WHERE Price < 2147486 AND Name LIKE '%';

INSERT INTO Cart (CustomerID, ProductID, Quantity) VALUES (97, 96, 2);

SELECT ProductID FRom Products WHERE Name = 'Herwoman';

SELECT p.Name, c.Quantity, p.Price FROM Cart c JOIN Products p on c.ProductID = p.ProductID WHERE CustomerID = 97;

SELECT SUM(c.Quantity * p.Price) FROM Cart c JOIN Products p on c.ProductID = p.ProductID WHERE CustomerID = 97;

SELECT c.Quantity, p.Quantity FROM Cart c JOIN Products p on c.ProductID = p.ProductID WHERE CustomerID = 97;

SELECT COUNT(*)
FROM Cart c JOIN Products p on c.ProductID = p.ProductID
WHERE c.Quantity > p.Quantity AND CustomerID = 97;

DELETE FROM Cart WHERE CustomerID = 97;

SELECT * FRom Orders WHERE CustomerID = 97;


SELECT COUNT(O.Unit_ID), ProductID
FROM (OrderedItems O JOIN Warehouse W on O.Unit_ID = W.UnitID)
WHERE O.OrderID = 97
GROUP BY ProductID;

SELECT TEMP.ProductID, TEMP.quantity, Name, Price FROM
    (SELECT COUNT(O.Unit_ID) AS quantity, ProductID
     FROM (OrderedItems O JOIN Warehouse W on O.Unit_ID = W.UnitID)
     WHERE O.OrderID = 97
     GROUP BY ProductID) as TEMP JOIN Products p on TEMP.ProductID = p.ProductID;

SELECT * FROM Orders WHERE OrderID = 97;

# chutiyap
ALTER TABLE Orders ADD COLUMN CustomerID int NOT NULL ;
UPDATE Orders SET Orders.CustomerID = OrderID;

SELECT MAX(UnitID) FROM Warehouse


