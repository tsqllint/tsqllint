CREATE TABLE dbo.t1 (
	Id INT,
	Name VARCHAR(64)
);

CREATE TABLE dbo.t2 (
	Id INT,
	Name VARCHAR(64)
);
GO;

--Partitioned view as defined on Server1  
CREATE VIEW dbo.PartitionedView  
AS  
	SELECT *  
	FROM t1
	UNION ALL
	SELECT *  
	FROM t2;
GO;

CREATE VIEW dbo.TestView
AS
  SELECT
	t1.Id AS t1_id,
	t1.Name AS t1_name
  FROM t1
  LEFT JOIN t2 ON t1.Id = t2.Id;
GO;

CREATE FUNCTION dbo.GetId (@name varchar(64))  
RETURNS int  
WITH EXECUTE AS CALLER  
AS  
BEGIN;
	DECLARE @returnVal int;
	SELECT @returnVal = t1.Id FROM t1 where t1.name = @name;
	RETURN @returnValue;
END;
GO;

CREATE FUNCTION dbo.SelectReturnTypeFunction ()
RETURNS TABLE
AS
RETURN
(
	WITH cte AS
	(
		SELECT c1, c2 FROM t1
	)

	SELECT c1, c2 FROM cte -- should not trigger an error
);
GO;

DROP VIEW dbo.PartitionedView;
DROP VIEW dbo.TestView;
DROP FUNCTION dbo.GetId;
DROP TABLE t1;
DROP TABLE t2;
DROP FUNCTION IF EXISTS dbo.SelectReturnTypeFunction;
GO;