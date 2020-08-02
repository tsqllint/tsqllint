# Disallows use of Full Text

## Rule Details

Use of improperly tuned full text queries can lead to performance problems

Examples of **incorrect** code for this rule:

```tsql
SELECT Name, ListPrice
FROM Production.Product
WHERE ListPrice = 80.99
   AND CONTAINS(Name, 'Mountain')
GO
```

Examples of **correct** code for this rule:

```tsql
SELECT Name, ListPrice
FROM Production.Product
WHERE ListPrice = 80.99
   AND Name like '%Mountain%'
GO
```
