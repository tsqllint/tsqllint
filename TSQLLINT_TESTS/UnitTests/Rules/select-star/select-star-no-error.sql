-- simple select
SELECT FOO FROM BAR;

-- select star within exists predicate
IF EXISTS (SELECT * from sys.triggers WHERE name='Foo' and type='TR')
BEGIN
	PRINT 'FOO'
END

-- select within exists predicate 
IF EXISTS (SELECT 1)
BEGIN
	PRINT 'FOO'
END