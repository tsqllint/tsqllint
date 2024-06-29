# Disallows use of Implicit Joins (Comma Joins)

## Rule Details

Use of comma joins is often considered an outdated style of syntax.  Under normal circumstances comma joins won't cause any accuracy or performance impact it is a best practice to explicitly declare "JOIN" conditions with the "ON" syntax as it is commonly mentioned that this style is preferred as it is more human readable.  

Examples of **incorrect** code for this rule:

```tsql
SELECT Name, ListPrice
FROM Production.Product p, Production.ProductCategory pc
WHERE 
   p.CategoryID = pc.CategoryID;
```

Examples of **correct** code for this rule:

```tsql
SELECT Name, ListPrice
FROM Production.Product p
INNER JOIN Production.ProductCategory pc
   ON p.CategoryID = pc.CategoryID;
```
