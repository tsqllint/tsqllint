# Disallows user of linked server calls

# Rule Details

This rule disallows linked server queries, which can cause table locking and are discouraged

Examples of **incorrect** code for this rule:

```sql
SELECT Foo
FROM MyServer.MyDatabase.MySchema.MyTable;
```

Examples of **correct** code for this rule:

```sql
SELECT Foo
FROM MyDatabase.MySchema.MyTable;
```
