BEGIN TRANSACTION;
    UPDATE DB1.dbo.Table1 SET Value = 1;
    UPDATE DB2.dbo.Table2 SET Value = 1;