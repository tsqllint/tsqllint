-- no rowset actions so nocount not required
CREATE TABLE RowSetActionTest ( 
  columnOne int, 
INDEX ix_1 NONCLUSTERED (columnOne))