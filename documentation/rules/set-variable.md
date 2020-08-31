# Enforce setting variable with SELECT statement

## Rule Details

This rule enforces setting variable values
using a `SELECT` statement.

Examples of **incorrect** code for this rule:

```tsql
DECLARE @MyVar VARCHAR(30);
SET @MyVar = 'This is incorrect.';
```

Examples of **correct** code for this rule:

```tsql
DECLARE @MyVar VARCHAR(30);
SELECT @MyVar = 'This is correct.';
```
