# Enforce setting SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED near top of file

## Rule Details

This rule enforces setting `SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED` near the top of a file.

Examples of **incorrect** code for this rule:

```tsql
SET @MyVar = 'This is incorrect.';
```

Examples of **correct** code for this rule:

```tsql
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
GO

SELECT @MyVar = 'This is correct.';
```
