SELECT Name, v.Name
FROM Production.Product
JOIN Purchasing.ProductVendor pv
    ON pv.ProductID = pv.ProductID
JOIN Purchasing.Vendor v
    ON pv.BusinessEntityID = v.BusinessEntityID
JOIN Sales.SalesOrderDetail sod
    ON sod.ProductID = pv.ProductID
JOIN Sales.SalesOrderHeader soh
    ON soh.SalesOrderID = sod.SalesOrderID;
