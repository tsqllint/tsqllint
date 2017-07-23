-- DDL Statements
CREATE TABLE MyTable 
	(ID INT, 
	 Name nvarchar)
WITH (DATA_COMPRESSION = ROW);

ALTER TABLE MyTable 
ADD Value nvarchar;

DROP TABLE MyTable;

TRUNCATE TABLE MyTable;