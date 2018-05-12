-- function call within fields, not predicate. should not error

SELECT 
Con.ID AS ContactId, 
Addr.Address
FROM dbo.Contact Con
INNER JOIN (
	SELECT ContactId, 
	Address, 
	ROW_NUMBER() OVER (PARTITION BY ContactId ORDER BY ID DESC) AS rownum
	FROM dbo.Address
) Addr ON Con.ID = Addr.ContactID
WHERE Addr.rownum = 1;

SELECT 
Con.ID AS ContactId, 
Addr.Address
FROM dbo.Contact Con
INNER JOIN (
	SELECT ContactId, 
	Address, 
	ROW_NUMBER() OVER (PARTITION BY ContactId ORDER BY ID DESC) AS rownum
	FROM dbo.Address
) Addr ON Con.ID = Addr.ContactID
WHERE Addr.rownum = 1;