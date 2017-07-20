SELECT FOO FROM BAR;
GO

-- should not be a violation
BEGIN TRY;  
    SELECT 1;
END TRY
BEGIN CATCH;  
    SELECT 2;
END CATCH;

-- should not be a violation
DECLARE @KeepGoing INT = 1;
WHILE (@KeepGoing = 1)
BEGIN;
  SELECT @KeepGoing = -1;
  PRINT 'Foo';
END;

IF NOT EXISTS (SELECT * FROM FOO)
BEGIN
    SELECT 1;
END

CREATE TABLE t1 (
  ColumnOne int, 
  INDEX IX_ColumnOne NONCLUSTERED (ColumnOne));