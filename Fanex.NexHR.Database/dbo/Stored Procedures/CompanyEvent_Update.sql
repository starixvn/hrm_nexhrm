
CREATE PROCEDURE [dbo].[CompanyEvent_Update] @Id INT
	,@Title NVARCHAR(150)
	,@FromDate DATE
	,@ToDate DATE
	,@FromTime VARCHAR(8)
	,@ToTime VARCHAR(8)
	,@Location nvarchar(100)
	,@Description NVARCHAR(max)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160419@Harvey.Nguyen
Task: Update event
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
			NOT EXISTS (
				SELECT 1
				FROM dbo.CompanyEvent e WITH (NOLOCK)
				WHERE e.Id = @Id
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Can not find event for this Id'

		RETURN
	END

	UPDATE dbo.CompanyEvent WITH (ROWLOCK, UPDLOCK)
	SET Title = @Title
		,FromDate = @FromDate
		,ToDate = @ToDate
		,FromTime = @FromTime
		,Location = @Location
		,ToTime = @ToTime
		,Description = @Description
	WHERE Id = @Id

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
