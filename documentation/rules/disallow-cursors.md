# Disallows use of cursors

## Rule Details

Use of cursors introduces a pattern of row based operations, which less preferred to better performing set based operations

Examples of **incorrect** code for this rule:

```sql
DECLARE @id int 
DECLARE cursorT CURSOR
FOR
SELECT ProductId
FROM AdventureWorks2012.Production.Product
WHERE DaysToManufacture <= 1
 
OPEN cursorT 
FETCH NEXT FROM cursorT INTO @id
WHILE @@FETCH_STATUS = 0
BEGIN
          SELECT * FROM Production.ProductInventory
          WHERE ProductID=@id
          FETCH NEXT FROM cursorT INTO @id
END
CLOSE cursorT 
DEALLOCATE cursorT 
```

Examples of **correct** code for this rule:

```sql
-- Alternative to cursor statement, using join

SET STATISTICS TIME ON
SELECT * FROM Production.ProductInventory as pinv
INNER JOIN Production.Product as pp
ON pinv.ProductID=pp.ProductID
WHERE pp.DaysToManufacture <= 1
```
