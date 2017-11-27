DECLARE @NAME nvarCHAR(25);

SELECT @name + N'ab';
SELECT N'ab' + @name;

SELECT @name + @name;

SELECT CASE WHEN N'a' + N'b' = @name THEN 1 ELSE 0 END;

SELECT CASE WHEN @name + N'b' = N'ab' THEN 1 ELSE 0 END;

SELECT CASE WHEN @name + N'b' = @name THEN 1 ELSE 0 END;

SELECT 'Test'
FROM sys.tables
WHERE name = @name + N'b';

SELECT 'Test'
FROM sys.tables
WHERE name = N'a' + @name;

SELECT col1
WHERE @name = N'a' + N'b';

SELECT 'Test'
FROM sys.tables t
	JOIN sys.columns c
	ON t.object_id = c.object_id
	AND c.name = @name + N'b';