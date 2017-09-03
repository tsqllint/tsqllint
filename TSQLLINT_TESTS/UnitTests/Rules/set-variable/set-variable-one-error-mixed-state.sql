DECLARE @var1 varchar(30)

SELECT @var1 = 'foo'

-- should error
SET @var1 = 'bar'