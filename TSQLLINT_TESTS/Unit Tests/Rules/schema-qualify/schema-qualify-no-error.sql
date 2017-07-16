-- schema and database qualifiers
SELECT DBO.SOMEDATABASE.FOO.BAR FROM DBO.SOMEDATABASE.FOO;

-- schema qualifiers
CREATE TABLE [dbo].[MyTable]
    ([ID] INT, 
     [Name] nvarchar);

-- temp table without qualifiers
SELECT * from #foo

-- table aliases should not have schema validation enforced
UPDATE MyTable
SET MyTable.TITLE = 'TEST'
FROM dbo.SomeTable MyTable
WHERE MyTable.ID = 100;

SELECT c.CustomerID, s.Name
FROM Sales.Customer AS c
JOIN Sales.Store AS s
ON c.CustomerID = s.BusinessEntityID ;