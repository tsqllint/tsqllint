# Disallow use of unicode chars in non-unicode string

## Rule Details

This rule disallows using unicode characters in
strings that do not support unicode characters.

Examples of **incorrect** code for this rule:

```tsql
SET @MyVar VARCHAR(30) = '¡This is incorrect!';
```

Examples of **correct** code for this rule:

```tsql
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
GO

SELECT @MyVar NVARCHAR(30) = '¡This is correct!';
```
