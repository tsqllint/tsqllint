SELECT 'a' + 'b';

SELECT 'a' + 
  'b';

SELECT 'a' 
  + 'b';

SELECT 
  'a' 
  + 'b';

SELECT 'a' + 'b' + 'c';

SELECT CASE WHEN 'a' + 'b' = 'ab' THEN 1 ELSE 0 END;

SELECT 'Test'
FROM sys.tables
WHERE name = 'a' + 'b';

SELECT 'Test'
FROM sys.tables t
	JOIN sys.columns c
	ON t.object_id = c.object_id
	AND c.name = 'a' + 'b';

SELECT 'a' + 'b' col1, col2, 'c' + 'd' as col3;

SELECT col1
WHERE 'a' + 'b' = 'ab';

SELECT col1
WHERE 'ab' = 'a' + 'b';