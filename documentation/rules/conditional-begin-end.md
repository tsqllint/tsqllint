# Enforce use of BEGIN and END symbols inside condition statements

## Rule Details

Use BEGIN and END to bind conditional statments as a single block of code.

Examples of **incorrect** code for this rule:
        
```sql
  IF (@parm = 1)
  BEGIN
    SELECT @output = ‘foo’
  END
```

Examples of **correct** code for this rule:

```sql
  IF (@parm = 1)
    SELECT @output = ‘foo’
```
