DECLARE @var1 varchar(30)
SET @var1 = 'foo'

DECLARE my_cursor CURSOR GLOBAL
FOR SELECT * FROM Foo;
DECLARE @my_variable CURSOR;
SET @my_variable = my_cursor;