CREATE PROCEDURE [dbo].[Bio_GetPendingFingerprintAndCard]
	
AS
/*
Created: 20180806@Oliver.Dang
Task: Get Fingerprint and card summary.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #FingerprintWarningUsers(UserId INT, IsWarning BIT)

	INSERT INTO #FingerprintWarningUsers(UserId, IsWarning)
	SELECT t.nUserIdn as UserId
			,1 as IsWarning
	FROM [BioStar20180607].dbo.TB_USER_TEMPLATE as t with (nolock) 
	LEFT JOIN dbo.BioFingerprint as f with (nolock) 
		ON f.nUserIdn = t.nUserIdn
		 AND f.nIndex = t.nIndex 
		 AND f.nTemplateIndex = t.nTemplateIndex
	WHERE f.EmpId IS NULL 
		OR f.[Status] = 0
		OR f.bTemplate <> t.bTemplate

	GROUP BY t.nUserIdn

	CREATE TABLE #CardWarningUsers(UserId INT, IsWarning BIT)

	INSERT INTO #CardWarningUsers(UserId, IsWarning)
	SELECT DISTINCT c.nUserIdn, 1
	FROM [BioStar20180607].dbo.TB_USER_CARD as c with (nolock) 
	LEFT JOIN dbo.BioCard as bc with (nolock) 
		ON bc.nCardIdn = c.nCardIdn
	WHERE (bc.EmpId IS NULL OR bc.[Status] = 0) AND (c.sCardNo != '0' AND c.nLatest = 1 AND c.nStatus = 1)
	GROUP BY c.nUserIdn
	
	SELECT 
		(ISNULL(fw.IsWarning, 0)) as IsFingerprintWarning
		,(ISNULL(cw.IsWarning, 0)) as IsCardWarning
		,e.Id AS EmpId
		,e.Code 
		,e.FullName
		,e.NickName
	FROM 
		 [dbo].Employee AS e WITH (NOLOCK)
		 JOIN [BioStar20180607].[dbo].[TB_USER] AS bu WITH (NOLOCK) ON '0' + bu.sUserID = e.Code
		 LEFT JOIN #FingerprintWarningUsers as fw WITH (NOLOCK) ON fw.UserId = bu.nUserIdn
		 LEFT JOIN #CardWarningUsers as cw WITH (NOLOCK) ON cw.UserId = bu.nUserIdn

	WHERE 
		e.[Enabled] = 1 
		AND e.[isDeleted] = 0
		AND (fw.IsWarning IS NOT NULL OR cw.IsWarning IS NOT NULL)

	GROUP BY bu.nUserIdn 
		,e.Id 
		,e.Code 
		,e.FullName
		,e.NickName
		,fw.IsWarning
		,cw.IsWarning

	DROP TABLE #CardWarningUsers
	DROP TABLE #FingerprintWarningUsers

END