# Enforce setting SET NOCOUNT ON near top of file

## Rule Details

This rule enforces setting `SET NOCOUNT ON`
near the top of the file.

Examples of **incorrect** code for this rule:

```tsql
DECLARE @MyVar VARCHAR(30);
SELECT @MyVar = 'This is incorrect.';
```

Examples of **correct** code for this rule:

```tsql
SET NOCOUNT ON;

DECLARE @MyVar VARCHAR(30);
SELECT @MyVar = 'This is correct.';
```
