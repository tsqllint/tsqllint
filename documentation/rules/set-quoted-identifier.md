# Enforce setting SET QUOTED_IDENTIFIER ON near top of file

## Rule Details

This rule enforces setting `SET QUOTED_IDENTIFIER ON`
near the top of the file.

Examples of **incorrect** code for this rule:

```tsql
SET @MyVar = 'This is incorrect.';
```

Examples of **correct** code for this rule:

```tsql
SET QUOTED_IDENTIFIER ON;
GO

SELECT @MyVar = 'This is correct.';
```
