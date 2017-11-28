SELECT ISNULL(Name, 0) FROM Foo;
SELECT * FROM Foo WHERE Foo.name = UPPER('foo');
SELECT * FROM Foo WHERE UPPER('foo') = Foo.name;

SELECT Name
FROM Production.Product
WHERE ProductSubcategoryID IN
    (SELECT ProductSubcategoryID
     FROM Production.ProductSubcategory
     WHERE UPPER('foo') = Foo.name);

-- no errors with isnull and an additional where clause
SELECT Name FROM Foo 
WHERE Foo.Value = 'Active'
AND ISNULL(ID, 0) != 0

SELECT Name FROM Foo 
WHERE ISNULL(ID, 0) != 0
AND Foo.Value = 'Active'

SELECT Name FROM Foo 
WHERE ISNULL(ID, 0) != 0
AND Foo.Value like '%active%'

SELECT Name FROM Foo 
WHERE ISNULL(ID, 0) != 0
AND Foo.Value BETWEEN 1 AND 100;

SELECT Name FROM Foo 
WHERE ISNULL(ID, 0) != 0
AND Foo.Value IN (1, 2, 3, 4, 5)

SELECT Name FROM Foo 
WHERE ISNULL(ID, 0) != 0
AND Foo.Value > 100;

SELECT Name FROM Foo 
INNER JOIN BAR ON ISNULL(Foo.ID, 0) = BAR.ID
AND Foo.Value > 100;