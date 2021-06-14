# Enforce use of WHERE condition when using DELETE

## Rule Details

This rule enforces the use of a WHERE condition in DELETE statements.

Examples of **incorrect** code for this rule:

```tsql
DELETE
FROM dbo.MyTable;
```

Examples of **correct** code for this rule:

```tsql
DELETE
FROM dbo.MyTable
WHERE CreatedOn = GETDATE();
```
