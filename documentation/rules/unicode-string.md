# Disallow use of unicode chars in non-unicode string

## Rule Details

This rule disallows using unicode characters in
strings that do not support unicode characters.

Examples of **incorrect** code for this rule:

```tsql
DECLARE @MyVar VARCHAR(30) = 'Ⱦhis is incorrect.';
```

Examples of **correct** code for this rule:

```tsql
DECLARE @MyVar NVARCHAR(30) = N'Ⱦhis is correct.';
```
