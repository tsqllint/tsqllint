SELECT p.Name, v.Name
FROM Production.Product p
JOIN Purchasing.ProductVendor pv
	ON p.ProductID = pv.ProductID
JOIN Purchasing.Vendor v
	ON pv.BusinessEntityID = v.BusinessEntityID
WHERE ProductSubcategoryID = 15
ORDER BY v.Name;