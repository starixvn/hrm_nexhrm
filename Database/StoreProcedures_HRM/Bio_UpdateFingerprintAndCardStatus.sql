USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Bio_UpdateFingerprintAndCardStatus]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Bio_UpdateFingerprintAndCardStatus]
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
	CREATE TABLE #TmpFingerprint(UserId INT, nIndex INT, TMPL0 image, TMPL1 image, nTemplateIndex INT)

	INSERT INTO #TmpFingerprint(UserId, nIndex, TMPL0, TMPL1, nTemplateIndex)
	SELECT t.USRUID,
		 t.FNGPUID, 
		 t.TMPL0, 
		 t.TMPL1, 
		 t.FNGPIDX
	FROM [ACDB].[dbo].[T_FNGPTMPL] AS t WITH (NOLOCK)
	WHERE t.USRUID = @BioUserId

	UPDATE b WITH (ROWLOCK, UPDLOCK)
	SET 
		b.[Status] = @Status
		,b.[TMPL0] = tf.TMPL0
		,b.[TMPL1] = tf.TMPL1
		,b.[LastUpdatedAt] = dbo.fnc_GetClientDateTime()
		,b.[LastUpdatedBy] = @UpdatedBy
	FROM dbo.BioFingerprint AS b WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #TmpFingerprint AS tf WITH (NOLOCK) ON tf.UserId = b.USRUID
	WHERE tf.UserId = @BioUserId AND tf.nIndex = b.FNGPUID AND tf.nTemplateIndex = b.FNGPIDX

	INSERT INTO dbo.BioFingerprint
		([EmpId]
		,[FNGPUID]
		,[USRUID]
		,[FNGPIDX]
		,[DUR]
		,[TMPL0]
		,[TMPL1]
		,[CHS]
		,[LSTMOD]
		,[Status]
		,[LastUpdatedAt]
		,[LastUpdatedBy])
	SELECT @EmployeeId
			,t.FNGPUID
			,t.USRUID
			,t.FNGPIDX
			,t.DUR
			,t.TMPL0
			,t.TMPL1
			,t.CHS
			,t.LSTMOD
			,@Status
			,dbo.fnc_GetClientDateTime()
			,@UpdatedBy
	FROM #TmpFingerprint as tf WITH (NOLOCK) 
		INNER JOIN [ACDB].[dbo].[T_FNGPTMPL] as t WITH (NOLOCK) 
			ON tf.UserId = t.USRUID AND tf.nIndex = t.FNGPUID AND tf.nTemplateIndex = t.FNGPIDX
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.BioFingerprint as b WITH (NOLOCK)
		WHERE tf.UserId = b.USRUID AND tf.nIndex = b.FNGPUID AND tf.nTemplateIndex = b.FNGPIDX
	) 

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update fingerprint and card failed.'

		RETURN
	END

	DROP TABLE #TmpFingerprint
	-- End Update status for fingerprint

	-- Begin Update status for card

	CREATE TABLE #TmpCard(UserId INT, CardIndex INT, CardMumber NVARCHAR(128))
	INSERT INTO #TmpCard(UserId, CardIndex, CardMumber)
	SELECT c.USRUID, c.CRDUID, b.CRDCSN
	FROM [ACDB].dbo.T_USRCRD as c WITH (NOLOCK)
	INNER JOIN [ACDB].[dbo].[T_CRD] as b
			ON b.CRDUID = c.CRDUID
	WHERE c.USRUID = @BioUserId AND c.CRDUID != '0'

	DELETE bio
	FROM dbo.BioCard AS bio 
	INNER JOIN dbo.BioUserCard AS bc ON bio.CRDUID = bc.CRDUID
	WHERE bc.USRUID = @BioUserId
	AND NOT EXISTS (SELECT 1 FROM #TmpCard WHERE CardIndex = bio.CRDUID)

	DELETE bc
	FROM dbo.BioCard AS bio 
	INNER JOIN dbo.BioUserCard AS bc ON bio.CRDUID = bc.CRDUID
	WHERE bc.USRUID = @BioUserId
	AND NOT EXISTS (SELECT 1 FROM #TmpCard WHERE CardIndex = bio.CRDUID)

	IF EXISTS (
		SELECT 1 
		FROM #TmpCard as tc 
		INNER JOIN dbo.BioUserCard AS bc WITH (NOLOCK) ON tc.UserId = bc.USRUID AND tc.CardIndex = bc.CRDUID
		INNER JOIN dbo.BioCard AS bio WITH (NOLOCK) ON bio.CRDUID = bc.CRDUID)
	BEGIN

		UPDATE bio WITH (ROWLOCK, UPDLOCK)
		SET bio.CRDCSN = tc.CardMumber
		FROM #TmpCard as tc 
		INNER JOIN dbo.BioUserCard AS bc WITH (NOLOCK) ON tc.UserId = bc.USRUID AND tc.CardIndex = bc.CRDUID
		INNER JOIN dbo.BioCard AS bio ON bio.CRDUID = bc.CRDUID AND tc.CardMumber = bio.CRDCSN

	END 

	INSERT INTO dbo.BioUserCard
			([EmpId]
			,[USRUID]
			,[CRDUID]
			,[IDX]
			,[ISSDDT]
			,[EXPDT]
			,[ISSDCNT]
			,[CRDMSK]
			,[PRDSTDT]
			,[PRDEDDT]
			,[Status]
			,[LastUpdatedAt]
			,[LastUpdatedBy])
		SELECT @EmployeeId
				,c.[USRUID]
				,c.[CRDUID]
				,c.[IDX]
				,c.[ISSDDT]
				,c.[EXPDT]
				,c.[ISSDCNT]
				,c.[CRDMSK]
				,c.[PRDSTDT]
				,c.[PRDEDDT]
				,@Status
				,dbo.fnc_GetClientDateTime()
				,@UpdatedBy
		FROM [ACDB].[dbo].[T_USRCRD] as c WITH (NOLOCK)
		INNER JOIN #TmpCard AS tc ON c.USRUID = tc.UserId AND c.CRDUID = tc.CardIndex
		WHERE NOT EXISTS 
			(SELECT 1 FROM dbo.BioUserCard WITH (NOLOCK) 
				WHERE UserId = tc.UserId
				AND CRDUID = tc.CardIndex
			)

	INSERT INTO dbo.BioCard
			([CRDUID]
			,[CRDTYPUID]
			,[CRDCSN]
			,[ASMUID]
			,[WGDFMUID]
			,[WGDFMTVER])
		SELECT c.[CRDUID]
			,c.[CRDTYPUID]
			,c.[CRDCSN]
			,c.[ASMUID]
			,c.[WGDFMUID]
			,c.[WGDFMTVER]
		FROM #TmpCard AS tc
		INNER JOIN [ACDB].[dbo].[T_CRD] as c WITH (NOLOCK) ON tc.CardIndex = c.CRDUID
		WHERE NOT EXISTS 
			(SELECT 1 FROM dbo.BioCard WITH (NOLOCK)
			WHERE CRDUID = tc.CardIndex)

	UPDATE bc WITH (ROWLOCK, UPDLOCK)
		SET bc.[Status] = @Status
			,bc.[LastUpdatedAt] = dbo.fnc_GetClientDateTime()
			,bc.[LastUpdatedBy] = @UpdatedBy
		FROM #TmpCard as tc 
		INNER JOIN dbo.BioUserCard AS bc ON tc.UserId = bc.USRUID AND tc.CardIndex = bc.CRDUID
		INNER JOIN dbo.BioCard AS bio WITH (NOLOCK) ON bio.CRDUID = bc.CRDUID AND tc.CardMumber = bio.CRDCSN

	DROP TABLE #TmpCard
	
END
GO
