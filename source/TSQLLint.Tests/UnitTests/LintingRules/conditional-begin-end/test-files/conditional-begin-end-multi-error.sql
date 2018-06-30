IF(1 = 1)
BEGIN
    SELECT 1
END

IF(1 = 1)
    SELECT 1

IF @foo IS NULL
    BEGIN
	    SELECT @Foo = NEWID();
    END;
ELSE
    SELECT @Foo = @Bar;