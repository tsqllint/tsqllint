/* should error */
SELECT TABLE_CATALOG FROM SomeDatabase.INFORMATION_SCHEMA.COLUMNS;

/* this is a valid when used without DB Identifier*/
SELECT TABLE_CATALOG FROM INFORMATION_SCHEMA;