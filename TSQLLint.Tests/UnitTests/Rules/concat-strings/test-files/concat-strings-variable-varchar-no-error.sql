DECLARE @NAME varCHAR(25);

SELECT @name + 'ab';
SELECT 'ab' + @name;

SELECT @name + @name;

SELECT CASE WHEN 'a' + 'b' = @name THEN 1 ELSE 0 END;

SELECT CASE WHEN @name + 'b' = 'ab' THEN 1 ELSE 0 END;

SELECT CASE WHEN @name + 'b' = @name THEN 1 ELSE 0 END;

SELECT 'Test'
FROM sys.tables
WHERE name = @name + 'b';

SELECT 'Test'
FROM sys.tables
WHERE name = 'a' + @name;

SELECT col1
WHERE @name = 'a' + 'b';

SELECT 'Test'
FROM sys.tables t
	JOIN sys.columns c
	ON t.object_id = c.object_id
	AND c.name = @name + 'b';