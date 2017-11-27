SELECT 1 + 1;

SELECT @val + @val;

SELECT @val++;

SELECT @val = @val + 1;

SELECT CAST('1/1/2017') + @date;

IF (@ROWCNT != 0)
BEGIN
	PRINT 'Hi';
END;