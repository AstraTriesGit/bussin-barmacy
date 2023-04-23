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

SELECT * FRom information_schema.TABLES WHERE TABLE_SCHEMA = 'pharmacy';

USE pharmacy_prod;
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

INSERT INTO Products (ProductID, Name, Manufacturer, Price, Quantity) VALUES (78, 'Bro', 'Chutiya', 32110, 90);

DELETE FROM Products WHERE ProductID = 97;

UPDATE Products SET Quantity = 90 WHERE ProductID = 97;

SELECT Quantity FROM Products WHERE ProductID = 97;

SELECT CustomerID, ProductID, Quantity FROM Cart WHERE CustomerID = 97;

SELECT MAX(OrderID) FROM Orders;

SELECT CorrespondentID FROM Orders WHERE Status = 'Delivered';

# chutiyap
ALTER TABLE Orders ADD COLUMN CustomerID int NOT NULL ;
UPDATE Orders SET Orders.CustomerID = OrderID;
UPDATE Products SET Quantity = Quantity / 3;

SELECT MAX(UnitID) + 10 FROM Warehouse;

CREATE PROCEDURE LoadNewProducts(
    IN qty int,
    IN prodId int
)
BEGIN
    DECLARE counter int default 1;
    SELECT MAX(UnitID) FROM Warehouse INTO @bro;
    START TRANSACTION;
    WHILE counter < qty + 1 DO
            INSERT INTO Warehouse (UnitID, DateOfAddition, ProductID) VALUES (
                                                                                     @bro + counter,
                                                                                     DATE(NOW()),
                                                                                     prodId
                                                                             );
            SET counter = counter + 1;
        end while;
    COMMIT;
end;

DROP PROCEDURE pharmacy.LoadNewProducts;

CALL LoadNewProducts(40, 97);

SELECT * FROM Warehouse WHERE ProductID = 97;

# get LIMIT items of particular prod id 
INSERT INTO OrderedItems (SELECT UnitID AS Unit_ID, 78 AS OrderId FROM Warehouse WHERE ProductID = 97 LIMIT 1);
SELECT ProductID, Quantity FROM Cart WHERE CustomerID = 97;


CREATE PROCEDURE DeleteProducts(
    IN qty int,
    IN prodId int
)
BEGIN
    DECLARE counter int default 0;
    START TRANSACTION;
    WHILE counter < qty DO
            DELETE FROM Warehouse WHERE UnitID = (SELECT MIN(UnitID) FROM Warehouse WHERE ProductID = prodId) + counter;
            SET counter = counter + 1;
        end while;
    COMMIT;
end;

CALL DeleteProducts(40, 97);

SELECT COUNT(OrderID) FROM pharmacy_prod.Orders;

INSERT INTO pharmacy_prod.Orders (OrderID, OrderDate, Status, Cost, PaymentType, CorrespondentID, CustomerID) VALUES
    ((SELECT COUNT(OrderID)) + 1, NOW(), 'Scheduled', 1000, 'COD', 3, 1);
SELECT * FROM Orders;

INSERT INTO pharmacy_prod.OrderedItems (SELECT UnitID AS Unit_ID,{orderId} AS OrderId FROM pharmacy_prod.Warehouse WHERE ProductID = {productId} LIMIT {qty})


