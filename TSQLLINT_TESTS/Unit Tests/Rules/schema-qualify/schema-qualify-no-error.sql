-- select query with schema and database qualifiers
SELECT DBO.SOMEDATABASE.FOO.BAR FROM DBO.SOMEDATABASE.FOO;

-- create table with schema qualifiers
CREATE TABLE [dbo].[MyTable]
    ([ID] INT, 
     [Name] nvarchar);

-- temp table without qualifiers
SELECT * from #foo

/**************************************************************
  Statements that are not required to have schema qualification
**************************************************************/

-- table aliases should not have schema validation enforced
UPDATE MyTable
    SET MyTable.TITLE = 'TEST'
    FROM dbo.SomeTable MyTable
WHERE MyTable.ID = 100;

SELECT c.CustomerID, s.Name
    FROM Sales.Customer AS c
    JOIN Sales.Store AS s
ON c.CustomerID = s.BusinessEntityID ;

-- CTE's should not have schema validation enforced
WITH Sales_CTE (SalesPersonID, SalesOrderID, SalesYear)
AS
(
    SELECT SalesPersonID, SalesOrderID, YEAR(OrderDate) AS SalesYear
    FROM Sales.SalesOrderHeader
    WHERE SalesPersonID IS NOT NULL
)
SELECT SalesPersonID, COUNT(SalesOrderID) AS TotalSales, SalesYear
FROM Sales_CTE
GROUP BY SalesYear, SalesPersonID
ORDER BY SalesPersonID, SalesYear;
GO

-- allow use of INSERTED in triggers
CREATE TRIGGER dbo.TR_Foo ON dbo.Foo  
INSTEAD OF INSERT  
AS  
BEGIN  
    INSERT INTO dbo.Foo (Name, ModifiedDate)  
        OUTPUT INSERTED.Name, INSERTED.ModifiedDate
    SELECT Name, getdate()  
    FROM inserted;  
END