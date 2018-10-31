CREATE PROCEDURE [DepartmentTests].[Test Department_Update procedure exists]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.Department_Update'
END