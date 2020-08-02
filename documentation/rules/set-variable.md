# Enforce setting variable with SELECT statement

## Rule Details

This rule enforces setting variable values
using a `SELECT` statement.

Examples of **incorrect** code for this rule:

```tsql
SET @MyVar VARCHAR(30) = 'This is incorrect.';
```

Examples of **correct** code for this rule:

```tsql
SELECT @MyVar VARCHAR(30) = 'This is correct.';
```
