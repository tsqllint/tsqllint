SELECT * FROM Foo INNER JOIN Bar on UPPER(Foo.name) = UPPER(Bar.name);

SELECT * FROM Foo INNER JOIN Bar on UPPER(Foo.name) = 'FOO';

SELECT * FROM Foo JOIN Bar on UPPER(Foo.name) = 'FOO';

SELECT * FROM Foo WHERE UPPER(Foo.name) = 'FOO';

SELECT Name FROM Foo WHERE LEFT(Foo.Name, 1) = 'A';

SELECT * FROM FOO WHERE CONVERT(CHAR(10), CreatedDate, 121) = '2017-08-19';

SELECT * FROM FOO WHERE ISNUMERIC(Foo.Value) = 1;

SELECT * FROM FOO WHERE DATEDIFF(millisecond, Foo.CreatedDate, GETDATE()) > 1000;

SELECT * FROM FOO WHERE MyFunction(Foo.Value) = 1;

SELECT Name FROM Foo WHERE RIGHT(Foo.Name, 1) = 'A';

SELECT Name FROM Foo WHERE CAST(Foo.CreatedDateString AS datetime) > GETDATE();

-- with spaces
SELECT Name
FROM Production.Product
WHERE ProductSubcategoryID IN
    (SELECT ProductSubcategoryID
     FROM Production.ProductSubcategory
     WHERE UPPER(Foo.name) = 'Foo');

-- with tabs
SELECT Name
FROM Production.Product
WHERE ProductSubcategoryID IN
	(SELECT ProductSubcategoryID
	FROM Production.ProductSubcategory
	WHERE UPPER(Foo.name) = 'Foo');

-- with inline tabs
SELECT Name
FROM Production.Product
WHERE ProductSubcategoryID IN
	(SELECT ProductSubcategoryID
	FROM Production.ProductSubcategory
	WHERE	UPPER(Foo.name) = 'Foo');

-- with tabs on newline
SELECT Name
FROM Production.Product
WHERE ProductSubcategoryID IN
	(SELECT ProductSubcategoryID
	FROM Production.ProductSubcategory
	WHERE
	    	UPPER(Foo.name) = 'Foo');