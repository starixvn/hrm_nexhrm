CREATE PROCEDURE [DepartmentTests].[Test Department_Delete procedure exists]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.Department_Delete'
END
