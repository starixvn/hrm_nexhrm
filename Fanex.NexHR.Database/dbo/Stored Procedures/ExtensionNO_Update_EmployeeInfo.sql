
CREATE PROCEDURE [dbo].[ExtensionNO_Update_EmployeeInfo] @ExtID INT
	,@FullName NVARCHAR(200) = NULL
	,@NickName NVARCHAR(100) = NULL
	,@ExtensionNo NVARCHAR(200)
	,@IsInternal BIT
	,@EmpID INT = NULL
	,@GroupID INT
	,@OrderNo SMALLINT = 1
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-01-04
Task: Update Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @IsGroup BIT = 0

	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE ExtID = @ExtID
				AND [IsGroup] = 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Parameter @ExtID not exists on systems.'

		RETURN
	END

	SET @IsInternal = ISNULL(@IsInternal, 1)

	IF @IsInternal = 1
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM dbo.Employee WITH (NOLOCK)
				WHERE id = @EmpID
				)
		BEGIN
			SELECT @ErrCode = 101
				,@ErrMsg = N'Parameter EmpID not exists on systems.'

			RETURN
		END
				--IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[ExtensionNumbers] exg WITH(NOLOCK) WHERE ExtID = @GroupID AND [IsGroup]=1 AND IsInternal = 1)
				--BEGIN
				--    SELECT @ErrCode = 102, @ErrMsg = N'Parameter GroupID not exists on systems.'
				--    RETURN
				--END
	END
	ELSE
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
				WHERE ExtID = @GroupID
					AND [IsGroup] = 1
					AND IsInternal = 0
				)
		BEGIN
			SELECT @ErrCode = 103
				,@ErrMsg = N'Parameter EmpID not exists on systems.'

			RETURN
		END
	END

	IF @IsInternal = 1
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
				WHERE [GroupID] = @GroupID
					AND [IsInternal] = 1
				)
		BEGIN
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
			SELECT N'' [FullName]
				,'' [NickName]
				,'' [ExtensionNo]
				,1 IsInternal
				,1 [IsGroup]
				,NULL [EmpID]
				,@GroupID [GroupID]
				,MAX([OrderNo])
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE [GroupID] <> @GroupID
				AND [IsInternal] = 1
		END
	END

	UPDATE [dbo].[ExtensionNumbers] WITH (ROWLOCK)
	SET [FullName] = @FullName
		,[NickName] = @NickName
		,[ExtensionNo] = @ExtensionNo
		,[IsInternal] = @IsInternal
		,[IsGroup] = 0
		,[EmpID] = CASE 
			WHEN @IsInternal = 1 THEN @EmpID
			ELSE NULL
			END
		,[GroupID] = @GroupID
		,[OrderNo] = @OrderNo
	WHERE ExtID = @ExtID
		AND [IsGroup] = 0

	SET @ErrCode = 0
END



