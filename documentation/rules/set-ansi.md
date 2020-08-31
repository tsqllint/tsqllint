# Enforce setting SET ANSI_NULLS ON near top of file

## Rule Details

This rule enforces setting `SET ANSI_NULLS ON`
near the top of the file.

Examples of **incorrect** code for this rule:

```tsql
DECLARE @MyVar VARCHAR(30);
SELECT @MyVar = 'This is incorrect.';
```

Examples of **correct** code for this rule:

```tsql
SET ANSI_NULLS ON;
GO

DECLARE @MyVar VARCHAR(30);
SELECT @MyVar = 'This is correct.';
```
