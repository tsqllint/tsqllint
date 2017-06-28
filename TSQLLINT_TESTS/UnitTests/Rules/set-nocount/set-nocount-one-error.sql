-- Rowset Actions
SELECT * FROM FOO;

UPDATE FOO SET BAR = 1;

DELETE FROM BAR;

INSERT INTO Production.UnitMeasure (Name, ID)
VALUES (N'Foo', 1);

-- DDL Statements
CREATE TABLE MyTable 
	(ID INT, 
	 Name nvarchar)
WITH (DATA_COMPRESSION = ROW);

ALTER TABLE MyTable 
ADD Value nvarchar;

DROP TABLE MyTable;

TRUNCATE TABLE MyTable;