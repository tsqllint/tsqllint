SELECT ISNULL(Name, 0) FROM Foo;
SELECT * FROM Foo WHERE Foo.name = UPPER('foo');
SELECT * FROM Foo WHERE UPPER('foo') = Foo.name;

SELECT Name
FROM Production.Product
WHERE ProductSubcategoryID IN
    (SELECT ProductSubcategoryID
     FROM Production.ProductSubcategory
     WHERE UPPER('foo') = Foo.name);