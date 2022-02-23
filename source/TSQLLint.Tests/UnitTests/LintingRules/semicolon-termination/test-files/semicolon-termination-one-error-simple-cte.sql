WITH CTE AS
(
	SELECT a, b FROM Foo.Baz
)

SELECT a, b FROM CTE -- should trigger an error