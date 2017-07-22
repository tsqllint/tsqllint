SELECT Name, Name
FROM Production.Product
JOIN Purchasing.ProductVendor
	ON ProductID = ProductID
JOIN Purchasing.Vendor
	ON BusinessEntityID = BusinessEntityID
WHERE ProductSubcategoryID = 15
ORDER BY Name;

SELECT p.Name, Name
	FROM Purchasing.Product p
JOIN Purchasing.ProductVendor pv
	ON ProductID = pv.ProductID
JOIN Purchasing.Vendor
	ON pv.BusinessEntityID = BusinessEntityID
WHERE ProductSubcategoryID = 15
ORDER BY Name;