CREATE TABLE MyTable 
	(ID INT, 
	Name nvarchar(50));

-- compression options on index not table
CREATE TABLE [#SomeTempTable](
	ID INT, 
	Name nvarchar(50)
UNIQUE(ID), 
PRIMARY KEY CLUSTERED (ID, Name) 
WITH (DATA_COMPRESSION = PAGE));