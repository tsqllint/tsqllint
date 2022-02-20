CREATE VIEW [foo].[bar] AS

SELECT a, b FROM foo.baz -- only one error should be triggered
