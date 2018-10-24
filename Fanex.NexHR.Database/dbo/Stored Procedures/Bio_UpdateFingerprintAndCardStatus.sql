
CREATE PROCEDURE [dbo].[Bio_UpdateFingerprintAndCardStatus]
	@BioUserId INT
	,@EmployeeId INT
	,@Status SMALLINT
	,@UpdatedBy INT
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180621@Oliver.Dang
Task: Update BioFingerprint and card status
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;
	
	-- Begin Update status for fingerprint
	CREATE TABLE #TmpFingerprint(UserId INT, nIndex INT, bTemplate BINARY(384), nTemplateIndex SMALLINT)

	INSERT INTO #TmpFingerprint(UserId, nIndex, bTemplate, nTemplateIndex)
	
	SELECT t.nUserIdn,
		 t.nIndex, 
		 t.bTemplate, 
		 t.nTemplateIndex
	FROM [BioStar].dbo.TB_USER_TEMPLATE as t with (nolock) 
	WHERE t.nUserIdn = @BioUserId
	GROUP BY t.nUserIdn
		,t.nIndex
		,t.bTemplate
		,t.nTemplateIndex

	UPDATE b WITH (ROWLOCK, UPDLOCK)
		SET 
			b.[Status] = @Status
			,b.[bTemplate] = tf.bTemplate
			,b.[LastUpdatedAt] = GETDATE()
			,b.[LastUpdatedBy] = @UpdatedBy
		FROM dbo.BioFingerprint AS b WITH (ROWLOCK, UPDLOCK)
		 INNER JOIN #TmpFingerprint AS tf WITH (NOLOCK) ON tf.UserId = b.nUserIdn
		WHERE tf.UserId = @BioUserId AND tf.nIndex = b.nIndex AND tf.nTemplateIndex = b.nTemplateIndex 

		INSERT INTO [dbo].[BioFingerprint]
			([EmpId]
			,[nUserIdn]
			,[nIndex]
			,[bTemplate]
			,[nTemplatecs]
			,[nDuress]
			,[nEncryption]
			,[nSecurityLevel]
			,[nFingerIndex]
			,[nTemplateIndex]
			,[nEnrollQuality]
			,[Status]
			,[LastUpdatedAt]
			,[LastUpdatedBy])
		SELECT @EmployeeId
				,t.nUserIdn
				,t.nIndex
				,t.bTemplate
				,t.nTemplatecs
				,t.nDuress
				,t.nEncryption
				,t.nSecurityLevel
				,t.nFingerIndex
				,t.nTemplateIndex
				,t.nEnrollQuality
				,@Status
				,GETDATE()
				,@UpdatedBy
		FROM #TmpFingerprint as tf WITH (NOLOCK) 
			INNER JOIN [BioStar].[dbo].[TB_USER_TEMPLATE] as t WITH (NOLOCK) ON tf.UserId = t.nUserIdn AND tf.nIndex = t.nIndex AND tf.nTemplateIndex = t.nTemplateIndex
		WHERE NOT EXISTS (SELECT 1 FROM dbo.BioFingerprint as b WITH (NOLOCK) WHERE tf.UserId = b.nUserIdn AND tf.nIndex = b.nIndex AND tf.nTemplateIndex = b.nTemplateIndex) 

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update fingerprint and card failed.'

		RETURN
	END


	DROP TABLE #TmpFingerprint
	-- End Update status for fingerprint

	-- Begin Update status for card

	CREATE TABLE #TmpCard(UserId INT, CardNo INT)
	INSERT INTO #TmpCard(UserId, CardNo)
	SELECT c.nUserIdn, c.sCardNo
	FROM [BioStar].dbo.TB_USER_CARD as c WITH (NOLOCK)
	WHERE c.nUserIdn = @BioUserId AND c.sCardNo != '0' AND c.nLatest = 1 AND c.nStatus = 1

	IF EXISTS (SELECT 1 
				FROM #TmpCard as tc LEFT JOIN BioCard as bc ON tc.UserId = bc.nUserIdn AND tc.CardNo = bc.sCardNo 
				WHERE bc.EmpId IS NOT NULL)
	BEGIN
	UPDATE [dbo].BioCard WITH (ROWLOCK, UPDLOCK)
	SET [Status] = @Status
		,[LastUpdatedAt] = GETDATE()
		,[LastUpdatedBy] = @UpdatedBy
	WHERE nUserIdn = @BioUserId
	END 
	ELSE
	BEGIN
		INSERT INTO [dbo].[BioCard]
			([EmpId]
			,[nCardIdn]
			,[nUserIdn]
			,[nType]
			,[sCardNo]
			,[sCustomNo]
			,[nSecurityLevel]
			,[nDateTime]
			,[nExpiryDateTime]
			,[nBypass]
			,[nStatus]
			,[nLatest]
			,[nEncryption]
			,[nV2CardType]
			,[Status]
			,[LastUpdatedAt]
			,[LastUpdatedBy])
		SELECT @EmployeeId
				,c.nCardIdn
				,c.nUserIdn
				,c.nType
				,c.sCardNo
				,c.sCustomNo
				,c.nSecurityLevel
				,c.nDateTime
				,c.nExpiryDateTime
				,c.nBypass
				,c.nStatus
				,c.nLatest
				,c.nEncryption
				,c.nV2CardType
				,@Status
				,GETDATE()
				,@UpdatedBy
		FROM [BioStar].[dbo].[TB_USER_CARD] as c WITH (NOLOCK)
		JOIN #TmpCard as tc WITH (NOLOCK) ON c.nUserIdn = tc.UserId AND c.sCardNo = tc.CardNo AND c.nStatus = 1
	END

	DROP TABLE #TmpCard

	-- End Update status for card

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update fingerprint and card failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = N'Update fingerprint and card successfully.'
	END
	
END
