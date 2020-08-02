# Disallow using UPPER/LOWER functions in comparisons

## Rule Details

This rule disallows using `UPPER` or `LOWER` functions
when performing comparisons since this is not required
in case insensitive databases and can affect query SARGability.

Examples of **incorrect** code for this rule:

```tsql
SELECT user_name
FROM dbo.MyTable
WHERE UPPER(first_name) = 'NATHAN';
```

Examples of **correct** code for this rule:

```tsql
SELECT user_name
FROM dbo.MyTable
WHERE first_name = 'NATHAN';
```
