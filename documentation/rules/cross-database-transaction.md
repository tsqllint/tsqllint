# Discourages inserts or updates that create transactions in more than one database.

## Rule Details

Cross database inserts or updates enclosed in a transaction can lead to data corruption

Examples of **incorrect** code for this rule:
        
```sql
  BEGIN TRANSACTION;
    UPDATE DB1.dbo.Table1 SET Value = 1;
    UPDATE DB2.dbo.Table2 SET Value = 1;
  COMMIT TRANSACTION;
```

Examples of **correct** code for this rule:

```sql
  BEGIN TRANSACTION;
    UPDATE DB1.dbo.Table1 SET Value = 1;
  COMMIT TRANSACTION;
  
  BEGIN TRANSACTION;
    UPDATE DB2.dbo.Table2 SET Value = 1;
  COMMIT TRANSACTION 
```
