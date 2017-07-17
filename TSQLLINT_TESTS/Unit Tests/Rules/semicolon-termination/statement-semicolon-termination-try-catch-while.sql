BEGIN TRY
	SELECT 1;
END TRY
BEGIN CATCH
	SELECT 2;
END CATCH -- missing semicolon

DECLARE @KeepGoing INT = 1;
WHILE (@KeepGoing = 1)
BEGIN -- missing semicolon
  SELECT @KeepGoing = -1;
  PRINT 'Foo';
END -- missing semicolon