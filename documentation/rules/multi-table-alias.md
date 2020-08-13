# Enforce aliasing in multi-table joins

## Rule Details

This rule enforces the use of table aliases
when multiple tables are joined in a query.

Examples of **incorrect** code for this rule:

```tsql
SELECT user_name, country_name
FROM dbo.MyTable
    INNER JOIN dbo.Country ON id = country_id;
```

Examples of **correct** code for this rule:

```tsql
SELECT mt.user_name, c.country_name
FROM dbo.MyTable AS mt
    INNER JOIN dbo.Country AS c ON c.id = mt.country_id;
```
