DECLARE @varname varchar(25),
		@nvarname nvarchar(25);

SELECT @varname + N'ab';
SELECT N'ab' + @varname;

SELECT @nvarname + 'ab';
SELECT 'ab' + @nvarname;

SELECT @varname + @nvarname;
SELECT @nvarname + @varname;

SELECT CASE WHEN N'a' + N'b' = @varname THEN 1 ELSE 0 END;

SELECT CASE WHEN @varname = N'a' + N'b' THEN 1 ELSE 0 END;

SELECT 'Test'
FROM sys.tables
WHERE name = @nvarname + 'b';

SELECT 'Test'
FROM sys.tables
WHERE name = @varname + N'b';

SELECT col1
WHERE @varname = N'a' + N'b';

SELECT col1
WHERE @nvarname = 'a' + 'b';

SELECT 'Test'
FROM sys.tables t
	JOIN sys.columns c
	ON t.object_id = c.object_id
	AND c.name = @nvarname + 'b';

SELECT 'Test'
FROM sys.tables t
	JOIN sys.columns c
	ON t.object_id = c.object_id
	AND c.name = @varname + N'b';