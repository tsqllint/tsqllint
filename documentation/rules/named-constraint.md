# Disallows named constraints in temp tables

## Rule Details

This rule disallows named constraints in temporary tables, which can cause collisions when used in parallel.

Examples of **incorrect** code for this rule:

```tsql
CREATE TABLE #temporary (
    ID INT IDENTITY (1,1),
    CreatedOn DATETIME2 NOT NULL
        CONSTRAINT [df_CreatedOn] DEFAULT GETDATE());
```

Examples of **correct** code for this rule:

```tsql
CREATE TABLE #temporary (
    ID INT IDENTITY (1,1),
    CreatedOn DATETIME2 NOT NULL DEFAULT GETDATE());
```
