# Disallows use of count-star

## Rule Details

Use of `COUNT(*)` is disallowed. Suggested alternatives are `COUNT(1)` or `COUNT(<PK>)`.

Examples of **incorrect** code for this rule:

```tsql
SELECT COUNT(*)
FROM Production.Product;
GO
```

Examples of **correct** code for this rule:

```tsql
SELECT COUNT(1)
FROM Production.Product;
GO
```

```tsql
SELECT COUNT([ProductID])
FROM Production.Product;
GO
```
