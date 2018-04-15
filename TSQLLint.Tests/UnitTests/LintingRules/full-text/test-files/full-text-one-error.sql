SELECT Name, ListPrice
FROM Production.Product
WHERE ListPrice = 80.99
   AND CONTAINS(Name, 'Mountain')
GO
