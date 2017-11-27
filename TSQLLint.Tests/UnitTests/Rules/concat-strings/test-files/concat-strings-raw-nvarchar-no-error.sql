SELECT N'a' + N'b';

SELECT N'a' + 
  N'b';

SELECT N'a' 
  + N'b';

SELECT 
  N'a' 
  + N'b';

SELECT N'a' + N'b' + N'c';

SELECT CASE WHEN N'a' + N'b' = N'ab' THEN 1 ELSE 0 END;

SELECT 'Test'
FROM sys.tables
WHERE name = N'a' + N'b';

SELECT col1
WHERE N'a' + N'b' = N'ab';

SELECT col1
WHERE N'ab' = N'a' + N'b';

SELECT 'Test'
FROM sys.tables t
	JOIN sys.columns c
	ON t.object_id = c.object_id
	AND c.name = N'a' + N'b';

SELECT N'a' + N'b' col1, col2, N'c' + N'd' as col3;
