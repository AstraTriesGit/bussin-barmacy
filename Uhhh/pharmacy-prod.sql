DROP SCHEMA IF EXISTS pharmacy_prod;
CREATE SCHEMA pharmacy_prod;
USE pharmacy_prod;

CREATE TABLE Customers(
    CustomerID int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    Name varchar(50) NOT NULL,
    Password varchar(50) NOT NULL,
    PhoneNumber varchar(15) NOT NULL,
    EMail varchar(50),
    Address varchar(150)
);


CREATE TABLE Products(
    ProductID int PRIMARY KEY NOT NULL AUTO_INCREMENT,
    Name varchar(50) NOT NULL,
    Manufacturer varchar(50) NOT NULL,
    Price decimal NOT NULL,
    Quantity int NOT NULL
);

CREATE TABLE Cart(
    SerialNo int PRIMARY KEY NOT NULL AUTO_INCREMENT,
    CustomerID int NOT NULL,
    ProductID int NOT NULL,
    Quantity int NOT NULL,

    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE DeliveryCorrespondents(
    CorrespondentID int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    isDelivering varchar(1) NOT NULL
);

CREATE TABLE Orders(
    OrderID int PRIMARY KEY NOT NULL AUTO_INCREMENT,
    OrderDate datetime NOT NULL,
    Status varchar(10) NOT NULL,
    Cost decimal NOT NULL,
    PaymentType varchar(25) NOT NULL,
    CorrespondentID int NOT NULL,
    CustomerID int NOT NULL,

    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (CorrespondentID) REFERENCES DeliveryCorrespondents(CorrespondentID)
);

CREATE TABLE Warehouse(
    UnitID int NOT NULL PRIMARY KEY,
    DateOfAddition date NOT NULL,
    ProductID int NOT NULL,

    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE OrderedItems(
    UnitID int NOT NULL PRIMARY KEY,
    FOREIGN KEY (UnitID) REFERENCES Warehouse(UnitID),
    OrderID int NOT NULL
);
ALTER TABLE OrderedItems ADD COLUMN ProductID int NOT NULL;


CREATE PROCEDURE LoadNewProducts(
    IN qty int,
    IN prodId int
)
BEGIN
    DECLARE counter int default 1;
    SELECT MAX(UnitID) FROM Warehouse INTO @bro;

    IF @bro IS NULL THEN SET @bro := 0;
    END IF;

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

CREATE PROCEDURE DeleteProducts(
    IN qty int,
    IN prodId int
)
BEGIN
    DECLARE counter int default 0;
    SELECT MIN(UnitID) FROM Warehouse WHERE ProductID = prodId INTO @real;
    START TRANSACTION;
    WHILE counter < qty DO
            DELETE FROM Warehouse WHERE UnitID = @`real` + counter;
            SET counter = counter + 1;
        end while;
    COMMIT;
end;

CREATE PROCEDURE AddToOrderedItems(
    IN qty int,
    IN oid int,
    IN prodID int
)
BEGIN
    DECLARE counter int default 0;
    SELECT MIN(UnitID) FROM Warehouse WHERE ProductID = prodId INTO @real;
    START TRANSACTION;
    WHILE counter < qty DO
            INSERT INTO OrderedItems (UnitID, OrderID) VALUES (@`real` + counter, oid);
            SET counter = counter + 1;
        end while;
    COMMIT;
end;

INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (1, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (2, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (3, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (4, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (5, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (6, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (7, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (8, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (9, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (10, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (11, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (12, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (13, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (14, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (15, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (16, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (17, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (18, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (19, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (20, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (21, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (22, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (23, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (24, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (25, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (26, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (27, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (28, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (29, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (30, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (31, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (32, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (33, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (34, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (35, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (36, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (37, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (38, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (39, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (40, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (41, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (42, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (43, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (44, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (45, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (46, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (47, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (48, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (49, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (50, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (51, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (52, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (53, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (54, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (55, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (56, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (57, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (58, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (59, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (60, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (61, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (62, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (63, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (64, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (65, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (66, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (67, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (68, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (69, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (70, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (71, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (72, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (73, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (74, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (75, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (76, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (77, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (78, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (79, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (80, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (81, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (82, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (83, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (84, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (85, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (86, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (87, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (88, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (89, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (90, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (91, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (92, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (93, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (94, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (95, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (96, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (97, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (98, '0');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (99, '1');
INSERT INTO `DeliveryCorrespondents` (`CorrespondentID`, `isDelivering`) VALUES (100, '0');



INSERT INTO Customers (Name, Password, PhoneNumber, EMail, Address) VALUES ('Anshu', 'ez', 72428467, 'fr@fr.com', 'IIIT DEL');

SELECT * FROM Products;
SELECT * FROM Warehouse;
SELECT * FROM Customers;
SELECT COUNT(OrderID) FROM pharmacy_prod.Orders;

SELECT * FROM Cart;

SELECT * FROM Orders;
TRUNCATE Orders;
SELECT * FROM OrderedItems;
SELECT * FROM Warehouse;
TRUNCATE OrderedItems;

SELECT * FROM DeliveryCorrespondents;

CALL AddToOrderedItems(10, 1, 2);


INSERT INTO pharmacy_prod.Cart (CustomerID, ProductID, Quantity) VALUES (1, 1, 10);

SELECT * FROM pharmacy_prod.Products WHERE Price < 200000 AND Name LIKE '%Vol%';

SELECT * FROM DeliveryCorrespondents WHERE isDelivering = '0';


