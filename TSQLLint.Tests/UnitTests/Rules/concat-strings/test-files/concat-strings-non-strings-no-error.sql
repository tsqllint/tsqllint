DECLARE @int int,
		@date datetime;

SELECT 1 + 1;

SELECT @int + @int;

SELECT @int++;

SELECT @int = @int + 1;

SELECT CAST('1/1/2017') + @date;

IF (@ROWCNT != 0)
BEGIN
	PRINT 'Hi';
END;

SELECT @int = 1 - 1;

SELECT '1' - '1';

SELECT @int = @int - 1;

SELECT @int = @int + 1 - 1;

SELECT @varname = @varname + 1 + '1';