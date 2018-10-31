CREATE PROCEDURE [AnnualLeaveTests].[Test LeaveTrans table exists]
AS
BEGIN
	EXEC tSQLt.AssertObjectExists 'dbo.LeaveTrans'
END
