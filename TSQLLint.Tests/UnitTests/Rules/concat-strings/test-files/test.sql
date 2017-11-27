SELECT 'Test'
FROM sys.tables t
	JOIN sys.columns c
	ON t.object_id = c.object_id
	AND c.name = N'a' + N'b';