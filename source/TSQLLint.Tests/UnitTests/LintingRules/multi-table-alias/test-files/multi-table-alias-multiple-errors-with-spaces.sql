SELECT Name, Name
FROM Production.Product
JOIN Purchasing.ProductVendor
    ON ProductID = ProductID
JOIN Purchasing.Vendor
    ON BusinessEntityID = BusinessEntityID
WHERE ProductSubcategoryID = 15
ORDER BY Name;