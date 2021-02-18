-- conditional-begin-end
IF(1 = 1)
	SELECT 1 FROM DBO.FOO;

-- data-compression
CREATE TABLE [dbo].[MyTable]
	([ID] INT, 
	 [Name] NVARCHAR(64));

-- data-type-length
CREATE TABLE MyTable 
	(ID INT, 
	 Name NVARCHAR)
WITH (DATA_COMPRESSION = ROW);

-- disallow-cursors
OPEN some_cursor;

-- information-schema
SELECT TABLE_CATALOG FROM INFORMATION_SCHEMA.COLUMNS;

-- keyword-capitalization
select Name FROM dbo.foo;

-- multi-table-alias
SELECT Name, v.Name
	FROM Purchasing.Product
JOIN Purchasing.ProductVendor pv
	ON ProductID = pv.ProductID
JOIN Purchasing.Vendor v
	ON pv.BusinessEntityID = v.BusinessEntityID
WHERE ProductSubcategoryID = 15
ORDER BY v.Name;

-- object-property
SELECT name, object_id, type_desc  
FROM sys.objects   
WHERE OBJECTPROPERTY(object_id, N'SchemaId') = SCHEMA_ID(N'Production')
ORDER BY type_desc, name;

-- print-statement
PRINT 'Foo';

-- schema-qualify
SELECT FOO FROM BAR;

-- select-star
SELECT * FROM dbo.BAR;

-- semicolon-termination & update-where
UPDATE [dbo].[FOO] SET BAR = 1

-- set-ansi
-- set-nocount
-- set-quoted-identifier
-- set-transaction-isolation-level

-- upper-lower & non-sargable
SELECT Value FROM dbo.Foo WHERE Value = upper("foo");