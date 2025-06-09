USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Bio_GetPendingFingerprintAndCard]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Bio_GetPendingFingerprintAndCard]
	
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
	SELECT t.USRUID as UserId
			,1
	FROM [ACDB].[dbo].[T_FNGPTMPL] as t with (nolock) 
	LEFT JOIN dbo.BioFingerprint as f with (nolock) 
		ON f.USRUID = t.USRUID AND f.FNGPUID = t.FNGPUID AND f.FNGPIDX = t.FNGPIDX
	WHERE f.[Status] = 0
		OR CONVERT(VARBINARY(384) , t.TMPL0) <> CONVERT(VARBINARY(384) , f.TMPL0) 
		OR CONVERT(VARBINARY(384) , t.TMPL1) <> CONVERT(VARBINARY(384) , f.TMPL1) 
	GROUP BY t.USRUID

	CREATE TABLE #CardWarningUsers(UserId INT, IsWarning BIT)

	INSERT INTO #CardWarningUsers(UserId, IsWarning)
	SELECT DISTINCT a.USRUID, 1
	FROM [ACDB].dbo.T_USRCRD as a with (nolock)	
	LEFT JOIN [HRM].dbo.BioUserCard as b with (nolock) 
		ON a.USRUID = b.USRUID AND a.CRDUID = b.CRDUID
	LEFT JOIN [ACDB].[dbo].[T_CRD] as c with (nolock)
		ON a.CRDUID = c.CRDUID
	LEFT JOIN [HRM].[dbo].[BioCard] as d with (nolock)
		ON d.CRDUID = b.CRDUID
	WHERE c.CRDCSN <> d.CRDCSN
	
	SELECT 
		(ISNULL(fw.IsWarning, 0)) as IsFingerprintWarning
		,(ISNULL(cw.IsWarning, 0)) as IsCardWarning
		,e.Id AS EmpId
		,e.Code 
		,e.FullName
		,e.NickName
	FROM 
		 [dbo].Employee AS e WITH (NOLOCK)
		 JOIN [ACDB].[dbo].[T_USR] AS bu WITH (NOLOCK) ON '0' + bu.USRID = e.Code
		 LEFT JOIN #FingerprintWarningUsers as fw WITH (NOLOCK) ON fw.UserId = bu.USRUID
		 LEFT JOIN #CardWarningUsers as cw WITH (NOLOCK) ON cw.UserId = bu.USRUID

	WHERE ISNULL(e.IsDeleted, 0) = 0
		AND (fw.IsWarning IS NOT NULL OR cw.IsWarning IS NOT NULL)

	GROUP BY bu.USRID 
		,e.Id 
		,e.Code 
		,e.FullName
		,e.NickName
		,fw.IsWarning
		,cw.IsWarning

	DROP TABLE #CardWarningUsers
	DROP TABLE #FingerprintWarningUsers

END
GO
