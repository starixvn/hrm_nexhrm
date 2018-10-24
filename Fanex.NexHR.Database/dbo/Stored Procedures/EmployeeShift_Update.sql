
/****** Object:  Stored Procedure dbo.EmployeeShift_Update    Script Date: 3/13/2016 3:21:41 PM ******/
-- =============================================
-- Author:        <Author,,Name>
-- Create date: <Create Date,,>
-- Description:    <Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[EmployeeShift_Update]
	-- Add the parameters for the stored procedure here
	@employeeId INT
	,@monShift INT = NULL
	,@tueShift INT = NULL
	,@wedShift INT = NULL
	,@thuShift INT = NULL
	,@friShift INT = NULL
	,@satShift INT = NULL
	,@sunShift INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @fromdate DATETIME
		,@dw INT
		,@isHoliday BIT

	SET @fromdate = GETDATE()

	-- Insert statements for procedure here
	UPDATE EmployeeShift
	SET MonShift = @monShift
		,TueShift = @tueShift
		,WedShift = @wedShift
		,ThuShift = @thuShift
		,FriShift = @friShift
		,SatShift = @satShift
		,SunShift = @sunShift
	WHERE EmpID = @employeeId

	UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
	SET @dw = datepart(dw, WorkDate)
		,@isHoliday = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.Holiday WITH (NOLOCK)
					WHERE WorkDate BETWEEN FromDate
							AND ToDate
					) THEN 1
			ELSE 0
			END
		,IsHoliday = @isHoliday
		,ShiftId = CASE 
			WHEN @dw = 1
				AND @isHoliday = 0 THEN @SunShift
			WHEN @dw = 2
				AND @isHoliday = 0 THEN @MonShift
			WHEN @dw = 3
				AND @isHoliday = 0 THEN @TueShift
			WHEN @dw = 4
				AND @isHoliday = 0 THEN @WedShift
			WHEN @dw = 5
				AND @isHoliday = 0 THEN @ThuShift
			WHEN @dw = 6
				AND @isHoliday = 0 THEN @FriShift
			WHEN @dw = 7
				AND @isHoliday = 0 THEN @SatShift
			ELSE ShiftId
			END
		,isOffDay = CASE 
			WHEN @dw = 1
				AND @SunShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 2
				AND @MonShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 3
				AND @TueShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 4
				AND @WedShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 5
				AND @ThuShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 6
				AND @FriShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 7
				AND @SatShift IS NULL
				AND @isHoliday = 0 THEN 1
			ELSE 0
			END
		,ModifiedDate = GETDATE()
	WHERE empId = @employeeId
		AND DATEDIFF(dd, @fromdate, WorkDate) >= 0
END
