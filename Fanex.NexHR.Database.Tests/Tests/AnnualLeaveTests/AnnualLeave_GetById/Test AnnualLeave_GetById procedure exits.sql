CREATE PROCEDURE [AnnualLeaveTests].[Test AnnualLeave_GetById procedure exits]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.AnnualLeave_GetById'
END
