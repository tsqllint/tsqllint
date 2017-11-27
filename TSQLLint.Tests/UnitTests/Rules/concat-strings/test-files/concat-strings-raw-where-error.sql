SELECT 'Test'
FROM sys.tables
WHERE name = N'a' + 'b';

SELECT 'Test'
FROM sys.tables
WHERE name = 'a' + N'b';

SELECT col1
WHERE 'a' + N'b' = N'ab';

SELECT col1
WHERE N'a' + 'b' = N'ab';

SELECT col1
WHERE 'a' + 'b' = N'ab';

SELECT col1
WHERE N'a' + 'b' = 'ab';

SELECT col1
WHERE 'a' + N'b' = 'ab';

SELECT col1
WHERE N'a' + N'b' = 'ab';

SELECT col1
WHERE N'ab' = N'a' + 'b';

SELECT col1
WHERE N'ab' = 'a' + N'b';

SELECT col1
WHERE N'ab' = 'a' + 'b';

SELECT col1
WHERE 'ab' = N'a' + 'b';

SELECT col1
WHERE 'ab' = 'a' + N'b';

SELECT col1
WHERE 'ab' = N'a' + N'b';

SELECT col1
WHERE N'ab' = N'a' + 'b';

SELECT col1
WHERE N'ab' = 'a' + N'b';

SELECT col1
WHERE N'ab' = 'a' + 'b';