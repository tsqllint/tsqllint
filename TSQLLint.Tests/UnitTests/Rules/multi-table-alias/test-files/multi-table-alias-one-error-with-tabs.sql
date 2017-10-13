SELECT Name, v.Name
	FROM Purchasing.Product
JOIN Purchasing.ProductVendor pv
	ON ProductID = pv.ProductID
JOIN Purchasing.Vendor v
	ON pv.BusinessEntityID = v.BusinessEntityID
WHERE ProductSubcategoryID = 15
ORDER BY v.Name;