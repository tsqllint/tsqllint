# Disallows use of INFORMATION_SCHEMA views

## Rule Details

The INFORMATION_SCHEMA are not the best source for object metadata, use SYS.OBJECTS

Examples of **incorrect** code for this rule:

```tsql
SELECT table_name FROM INFORMATION_SCHEMA.TABLES
WHERE table_schema = 'MyTable'
```

Examples of **correct** code for this rule:

```tsql
SELECT name
FROM sys.objects
WHERE OBJECTPROPERTY(object_id, N'SchemaId') = SCHEMA_ID(N'Production')
```
