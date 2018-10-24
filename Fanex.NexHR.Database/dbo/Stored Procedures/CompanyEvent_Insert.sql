
CREATE PROCEDURE [dbo].[CompanyEvent_Insert] @Title NVARCHAR(150)
	,@FromDate DATE
	,@ToDate DATE
	,@FromTime varchar(8)
	,@ToTime varchar(8)
	,@Location nvarchar(100)
	,@Description NVARCHAR(max)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160419@Harvey.Nguyen
Task: Insert new event
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.CompanyEvent e WITH (NOLOCK)
				WHERE FromDate = @FromDate
					AND e.ToDate = @ToDate
					AND e.Title = @Title
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'There are same event in date range.'

		RETURN
	END

	INSERT INTO dbo.CompanyEvent (
		Title
		,FromDate
		,ToDate
		,FromTime
		,ToTime
		,Location
		,Description
		,CreatedBy
		,CreatedOn
		)
	VALUES (
		@Title
		,@FromDate
		,@ToDate
		,@FromTime
		,@ToTime
		,@Location
		,@Description
		,@CreatedBy
		,getdate()
		)

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
