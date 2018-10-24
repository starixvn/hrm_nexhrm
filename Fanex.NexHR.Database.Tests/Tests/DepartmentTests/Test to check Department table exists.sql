CREATE PROCEDURE [DepartmentTests].[Test to check Department table exists]
AS
BEGIN
	EXEC tSQLt.AssertObjectExists 'dbo.Department'
END
