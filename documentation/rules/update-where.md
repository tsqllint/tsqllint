# Enforce use of WHERE condition when using UPDATE

## Rule Details

This rule enforces the use of a WHERE condition in UPDATE statements.

Examples of **incorrect** code for this rule:

```tsql
UPDATE mytable
SET CreatedOn = GETDATE()
FROM dbo.MyTable AS mytable;
```

Examples of **correct** code for this rule:

```tsql
UPDATE mytable
SET CreatedOn = GETDATE()
FROM dbo.MyTable AS mytable
WHERE CreatedOn IS NULL;
```
