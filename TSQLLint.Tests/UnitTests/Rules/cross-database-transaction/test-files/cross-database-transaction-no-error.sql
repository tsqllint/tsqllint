-- only queries that change data should be flagged
BEGIN TRANSACTION;
    SELECT Value FROM DB1.dbo.Table1;
    SELECT Value FROM DB2.dbo.Table2;
COMMIT TRANSACTION;

--3 part name, but updating same DB
BEGIN TRANSACTION;
    UPDATE DB1.dbo.Table1 SET Value = 1;
    UPDATE DB1.dbo.Table2 SET Value = 1;
COMMIT TRANSACTION;

-- tranaction without 3 part name
BEGIN TRANSACTION;
    UPDATE dbo.Table1 SET Value = 1;
    UPDATE dbo.Table2 SET Value = 1;
COMMIT TRANSACTION;

-- single update with 3 part name
BEGIN TRANSACTION;
    UPDATE DB1.dbo.Table1 SET Value = 1
COMMIT TRANSACTION;

-- 3 part name updates without transaction
UPDATE DB1.dbo.Table1 SET Value = 1
UPDATE DB2.dbo.Table2 SET Value = 1

BEGIN TRANSACTION
COMMIT TRANSACTION
UPDATE DB1.dbo.Table1 SET Value = 1
UPDATE DB2.dbo.Table2 SET Value = 1

-- begin transaction dont commit
BEGIN TRANSACTION
UPDATE DB1.dbo.Table1 SET Value = 1
UPDATE DB2.dbo.Table2 SET Value = 1

