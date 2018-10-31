CREATE PROCEDURE [DepartmentTests].[Test Department_Get procedure exists]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.Department_Get'
END