# Enforce terminating queries with semicolons

## Rule Details

This rule enforces termination of all queries with
a semicolon, as defined by Microsoft's
[Transact-SQL Syntax Conventions](https://docs.microsoft.com/en-us/sql/t-sql/language-elements/transact-sql-syntax-conventions-transact-sql?view=sql-server-ver15)

Examples of **incorrect** code for this rule:

```tsql
SELECT user_name
FROM dbo.MyTable
```

Examples of **correct** code for this rule:

```tsql
SELECT user_name
FROM dbo.MyTable;
```
