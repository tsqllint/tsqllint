-- uses of objectproperty should error
SELECT name, object_id, type_desc  
FROM sys.objects   
WHERE OBJECTPROPERTY(object_id, N'SchemaId') = SCHEMA_ID(N'Production')  
ORDER BY type_desc, name;  

-- statement not terminated with semicolon
UPDATE [dbo].[FOO] SET BAR = 1

-- tables are not schema qualified
SELECT FOO FROM BAR;

-- select star discouraged
SELECT * FROM dbo.BAR;

-- create table without compression or data length
CREATE TABLE [dbo].[MyTable]
    ([ID] INT, 
     [Name] nvarchar);

-- checking information schema rather than sys tables
SELECT TABLE_CATALOG FROM dbo.SomeDatabase.INFORMATION_SCHEMA.COLUMNS;

-- uses of upper lower should error
SELECT upper("foo");

-- script should start with these
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

-- print statements should not be allowed
PRINT 'Foo';

-- conditional blocks should contain being end blocks
IF(1 = 1)
    SELECT 1 FROM DBO.FOO;