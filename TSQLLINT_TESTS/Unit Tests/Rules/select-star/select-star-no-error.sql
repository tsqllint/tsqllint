--SELECT FOO FROM BAR;

IF EXISTS (SELECT * from sys.triggers WHERE name='Foo' and type='TR')
BEGIN
	PRINT 'FOO'
END