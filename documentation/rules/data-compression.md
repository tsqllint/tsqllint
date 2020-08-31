# Requires use of compression option during table creation

## Rule Details

Data compression can reduce database size and can help improve performance of I/O intensive workloads

Examples of **incorrect** code for this rule:

```tsql
  CREATE TABLE MyTable
    (ID INT,
    Name nvarchar(50))
```

Examples of **correct** code for this rule:

```tsql
  CREATE TABLE MyTable
    (ID INT,
    Name nvarchar(50))
  WITH (DATA_COMPRESSION = ROW);
```
