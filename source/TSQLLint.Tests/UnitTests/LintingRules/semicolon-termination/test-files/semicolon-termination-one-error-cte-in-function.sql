CREATE FUNCTION Foo.Bar ()
RETURNS TABLE

AS

RETURN
(
	WITH CTE AS
	(
		SELECT a, b FROM Foo.Baz
	)

	SELECT a, b FROM CTE -- should not trigger an error
) -- should trigger an error

GO