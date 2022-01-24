# Enforces common casing for variable references

## Rule Details

Variable names must use common casing when referenced multiple times.

Examples of **incorrect** code for this rule:

```tsql
DECLARE @ProductID INT = 25;
SELECT @PRODUCTID;
GO
```

Examples of **correct** code for this rule:

```tsql
DECLARE @ProductID INT = 25;
SELECT @ProductID;
GO
```
