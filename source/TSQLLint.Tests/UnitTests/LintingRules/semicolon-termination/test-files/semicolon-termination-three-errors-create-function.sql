CREATE FUNCTION Foo.Bar ()
RETURNS NVARCHAR(MAX)
AS
BEGIN

DECLARE @Baz NVARCHAR(MAX) -- should trigger an error
SELECT @Baz = 'Hello World' -- should trigger an error
RETURN @Baz -- should trigger an error

END;