CREATE PROCEDURE [DepartmentTests].[Test Department_Insert procedure exists]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.Department_Insert'
END