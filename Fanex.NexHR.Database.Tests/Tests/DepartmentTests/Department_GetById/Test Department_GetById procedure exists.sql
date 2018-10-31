CREATE PROCEDURE [DepartmentTests].[Test Department_GetById procedure exists]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.Department_GetById'
END