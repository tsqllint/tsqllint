SELECT Name, ListPrice
FROM Production.Product
WHERE ListPrice = 80.99
   AND CONTAINS(Name, 'Mountain')
GO

DECLARE @SearchWord varchar(30)
SET @SearchWord ='performance'
SELECT Description
FROM Production.ProductDescription
WHERE FREETEXT(Description, @SearchWord);
GO;
