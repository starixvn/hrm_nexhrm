CREATE PROCEDURE [DepartmentTests].[Test Department table exists]
AS
BEGIN
	EXEC tSQLt.AssertObjectExists 'dbo.Department'
END
