# Enforce qualifying objects with schema name

## Rule Details

This rule enforces qualifying references to objects with
schema names.

Examples of **incorrect** code for this rule:

```tsql
SELECT user_name
FROM MyTable;
```

Examples of **correct** code for this rule:

```tsql
SELECT user_name
FROM dbo.MyTable;
```
