-- schema and database qualifiers
SELECT DBO.SOMEDATABASE.FOO.BAR FROM DBO.SOMEDATABASE.FOO;

-- schema qualifiers
CREATE TABLE [dbo].[MyTable]
	([ID] INT, 
	 [Name] nvarchar);

-- temp table without qualifiers
SELECT * from #foo