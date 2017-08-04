SELECT p.Name, v.Name
FROM Production.Product p
JOIN Purchasing.ProductVendor pv
	ON p.ProductID = pv.ProductID
JOIN Purchasing.Vendor v
	ON pv.BusinessEntityID = v.BusinessEntityID
WHERE ProductSubcategoryID = 15
ORDER BY v.Name;
GO;

-- CTE's should not require alias
WITH MY_CTE (Foo, Bar)
AS (
	SELECT FooBars.Foo,
		FooCars.Bar
	FROM dbo.FooBars AS FooBars WITH (NOLOCK)
	LEFT JOIN dbo.FooCars AS FooCars
		ON Foobars.Foo = FooCars.Foo
)

SELECT *
FROM (
	SELECT *
	FROM dbo.SomeTable AS SomeTable WITH (NOLOCK)
	INNER JOIN MY_CTE  --multi-table-alias error here
		ON MYCTE.Foo = SomeTable.Foo
	WHERE SomeTable.ID < 5
) AS JoinedTable