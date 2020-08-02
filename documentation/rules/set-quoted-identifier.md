# Enforce setting SET QUOTED_IDENTIFIER ON near top of file

## Rule Details

This rule enforces setting `SET QUOTED_IDENTIFIER ON`
near the top of the file.

Examples of **incorrect** code for this rule:

```tsql
DECLARE @MyVar VARCHAR(30);
SELECT @MyVar = 'This is incorrect.';
```

Examples of **correct** code for this rule:

```tsql
SET QUOTED_IDENTIFIER ON;
GO

DECLARE @MyVar VARCHAR(30);
SELECT @MyVar = 'This is correct.';
```
