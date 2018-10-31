CREATE PROCEDURE [AnnualLeaveTests].[Test AnnualLeave_DeleteTransaction procedure exists]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.AnnualLeave_DeleteTransaction'
END