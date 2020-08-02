# Disallow functions on filter clauses or join predicates

## Rule Details

This rule disallows the use of functions on filter clauses
and join predicates, which can negatively impact performance
by making a query non-SARGable (Search ARGumentable).

Examples of **incorrect** code for this rule:

```tsql
SELECT user_name
FROM dbo.MyTable
WHERE DATEDIFF(day, created_on, GETDATE()) <= 3;
```

Examples of **correct** code for this rule:

```tsql
SELECT user_name
FROM dbo.MyTable
WHERE created_on >= DATEADD(day, -3, GETDATE());
```
