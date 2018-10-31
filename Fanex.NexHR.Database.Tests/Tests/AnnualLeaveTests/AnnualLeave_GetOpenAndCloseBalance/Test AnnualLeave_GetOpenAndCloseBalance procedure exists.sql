CREATE PROCEDURE [AnnualLeaveTests].[Test AnnualLeave_GetOpenAndCloseBalance procedure exists]
AS
BEGIN
   EXEC tSQLt.AssertObjectExists 'dbo.AnnualLeave_GetOpenAndCloseBalance'
END

