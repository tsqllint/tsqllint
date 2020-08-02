# Requires use of length when declaring data types with variable length

## Rule Details

Use of length when specifying data types can help with database size and improve I/O

Examples of **incorrect** code for this rule:

```tsql
  CREATE TABLE MyTable
    (ID INT,
     Name nvarchar);
```

Examples of **correct** code for this rule:

```tsql
  CREATE TABLE MyTable
  (ID INT,
  Name nvarchar(50))
```
