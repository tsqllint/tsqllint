# Disallow using ObjectedProperty function over sys view

## Rule Details

This rule disallows using the `ObjectProperty` function
when a sys view could be used instead.

Examples of **incorrect** code for this rule:

```tsql
IF OBJECTPROPERTY(OBJECT_ID(N'dbo.MyTable'),'ISTABLE') = 1
BEGIN
   SELECT 'dbo.MyTable is a table';
END
```

Examples of **correct** code for this rule:

```tsql
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MyTable')
BEGIN
   SELECT 'dbo.MyTable is a table';
END
```
