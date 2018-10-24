CREATE PROCEDURE [DepartmentTests].[Test to check Department_Insert procedure exists]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.Department_Insert'
END