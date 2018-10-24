
CREATE PROCEDURE [dbo].[ExtensionNO_Insert_GroupExternal] @FullName NVARCHAR(200) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-01-04
Task: Insert Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF @FullName IS NULL
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Group NAME invalid.'

		RETURN
	END

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE FullName = @FullName
				AND [IsGroup] = 1
				AND IsInternal = 0
			)
	BEGIN
		SELECT @ErrCode = 101
			,@ErrMsg = N'Group NAME exists on systems.'

		RETURN
	END

	DECLARE @GroupID INT

	INSERT INTO [ExtensionNumbers] (
		[FullName]
		,[NickName]
		,[ExtensionNo]
		,IsInternal
		,[IsGroup]
		,[EmpID]
		,[GroupID]
		,[OrderNo]
		)
	SELECT @FullName [FullName]
		,@FullName [NickName]
		,'' [ExtensionNo]
		,0 IsInternal
		,1 [IsGroup]
		,NULL [EmpID]
		,NULL [GroupID]
		,MAX([OrderNo])
	FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
	WHERE [GroupID] <> @GroupID
		AND [IsInternal] = 1

	SET @GroupID = cast(SCOPE_IDENTITY() AS INT)

	UPDATE [dbo].[ExtensionNumbers]
	SET GroupID = @GroupID
	WHERE ExtID = @GroupID

	SET @ErrCode = 0
END



