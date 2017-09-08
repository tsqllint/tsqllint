DECLARE @var1 varchar(30)

SELECT @var1 = 'foo'

SELECT @var1 = (
	SELECT Name   
	FROM Sales.Store
	WHERE CustomerID = 1000)

SELECT @var1 AS 'foo';

-- should not fail with update set statements
UPDATE FOO SET BAR = 1;