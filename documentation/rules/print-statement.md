# Disallow using PRINT statements

## Rule Details

This rule disallows using `PRINT` statements.

Examples of **incorrect** code for this rule:

```tsql
PRINT 'This is a debug message.'
```

Examples of **correct** code for this rule:

```tsql
RAISERROR('This is a debug message.', 160, 1);
```
