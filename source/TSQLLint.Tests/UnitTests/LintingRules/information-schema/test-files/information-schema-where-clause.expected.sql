IF NOT EXISTS (SELECT * FROM sys.procedures
	WHERE [object_id] = OBJECT_ID(N'dbo.sprocName'))
BEGIN
    SELECT 1;
END
GO

IF EXISTS (SELECT * FROM sys.tables 
	WHERE [object_id] = OBJECT_ID(N'dbo.tableName'))
BEGIN
	SELECT 1;
END


IF EXISTS (SELECT 1 FROM sys.columns 
	WHERE [object_id] = OBJECT_ID(N'dbo.tableName') AND [name] = 'columnName' AND [system_type_id] = TYPE_ID(N'nvarchar'))
BEGIN
SELECT 1;
END
