# Disallow selecting all columns with asterisk

## Rule Details

This rule disallows selecting all columns of a table
by using an asterisk.

Examples of **incorrect** code for this rule:

```tsql
SELECT *
FROM dbo.MyTable;
```

Examples of **correct** code for this rule:

```tsql
SELECT user_id, user_name, created_on
FROM dbo.MyTable;
```
