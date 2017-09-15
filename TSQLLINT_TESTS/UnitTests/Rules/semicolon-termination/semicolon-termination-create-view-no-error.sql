CREATE VIEW v_ExampleView
AS
  SELECT
	t1.Id AS t1_Id,
	t1.Name AS t1_Name,
	t1.T2Id AS t1_T2Id,
	t2.Name AS t2_Name
  FROM dbo.Table1 t1
  LEFT JOIN dbo.Table2 t2 ON t1.T2Id = t2.Id;
GO