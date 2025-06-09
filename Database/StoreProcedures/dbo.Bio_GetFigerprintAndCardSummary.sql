USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Bio_GetFigerprintAndCardSummary]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Bio_GetFigerprintAndCardSummary] 
	@EmployeeId INT = NULL
	,@GroupId INT = NULL
	,@Status SMALLINT = NULL -- true = aprroved
AS
/*
Created: 20180622@Oliver.Dang
Task: Get Fingerprint and card summary.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CompanyId INT

	SELECT @CompanyId = 1 FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId'

	CREATE TABLE #FingerprintWarningUsers(UserId INT, IsWarning BIT)

	INSERT INTO #FingerprintWarningUsers(UserId, IsWarning)
	
	SELECT t.USRUID as UserId
			,1
	FROM [ACDB].[dbo].[T_FNGPTMPL] as t with (nolock) 
	LEFT JOIN [HRM].dbo.BioFingerprint as f with (nolock) 
		ON f.USRUID = t.USRUID AND f.FNGPUID = t.FNGPUID AND f.FNGPIDX = t.FNGPIDX
	WHERE ISNULL(f.[Status], 0) = 0
		OR CONVERT(VARBINARY(384) , t.TMPL0) <> CONVERT(VARBINARY(384) , f.TMPL0) 
		OR CONVERT(VARBINARY(384) , t.TMPL1) <> CONVERT(VARBINARY(384) , f.TMPL1) 
	GROUP BY t.USRUID

	CREATE TABLE #CardWarningUsers(UserId INT, IsWarning BIT)

	INSERT INTO #CardWarningUsers(UserId, IsWarning)
	SELECT DISTINCT a.USRUID, 1
	FROM [ACDB].dbo.T_USRCRD as a with (nolock)	
	LEFT JOIN HRM.dbo.BioUserCard as b with (nolock) 
		ON a.USRUID = b.USRUID AND a.CRDUID = b.CRDUID
	LEFT JOIN [ACDB].[dbo].[T_CRD] as c with (nolock)
		ON a.CRDUID = c.CRDUID
	LEFT JOIN [HRM].[dbo].[BioCard] as d with (nolock)
		ON d.CRDUID = b.CRDUID
	WHERE c.CRDCSN <> ISNULL(d.CRDCSN, 0)
	--AND a.USRUID = 2047
	
	CREATE TABLE #FingerGroupByUserIdAndIndex
	(nUserId INT
	,nIndex SMALLINT)
	
	INSERT INTO #FingerGroupByUserIdAndIndex
	(nUserId, nIndex)
	SELECT 
		bt.USRUID
		,bt.FNGPUID
	FROM [ACDB].[dbo].[T_FNGPTMPL] AS bt
	GROUP BY bt.USRUID, bt.FNGPUID
		
	CREATE TABLE #FingerprintSummary
	(nUserId INT
	,Summary INT)
	
	INSERT INTO #FingerprintSummary(nUserId,Summary)
	SELECT
		fi.nUserId
		,COUNT(fi.nIndex)
	FROM #FingerGroupByUserIdAndIndex as fi with (nolock)
	GROUP BY fi.nUserId

	DROP TABLE #FingerGroupByUserIdAndIndex

	CREATE TABLE #CardSummary
	(nUserId INT
	,CardNo INT
	)
	
	INSERT INTO #CardSummary
	(nUserId, CardNo)
	SELECT uc.USRUID
		,COUNT(uc.CRDUID)
	FROM [ACDB].dbo.T_USRCRD AS uc WITH (NOLOCK)
		LEFT JOIN [ACDB].[dbo].T_CRD as crd WITH (NOLOCK)
			ON uc.CRDUID = crd.CRDUID
	WHERE uc.CRDUID != '0' AND crd.CRDTYPUID = 1
	GROUP BY uc.USRUID

	SELECT bu.USRUID AS BioUserId
		,SUM(CASE
			WHEN f.Summary IS NOT NULL THEN f.Summary
			ELSE 0
			END) AS TotalFingerprint
		,(CASE
			WHEN c.CardNo = '0' THEN NULL
			ELSE c.CardNo
			END) as CardNo
		,(ISNULL(fw.IsWarning, 0)) as IsFingerprintWarning
		,(ISNULL(cw.IsWarning, 0)) as IsCardWarning
		,e.Id AS EmpId
		,e.Code 
		,e.FullName
		,e.NickName
		,g.Code AS TeamCode
	FROM 
		 dbo.Employee AS e WITH (NOLOCK)
		 INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		 INNER JOIN [ACDB].[dbo].[T_USR] AS bu WITH (NOLOCK) ON '0' + bu.USRID = e.Code
		 LEFT JOIN #FingerprintSummary AS f WITH (NOLOCK) ON f.nUserId = bu.USRUID
		 LEFT JOIN #CardSummary AS c WITH (NOLOCK) ON c.nUserId = bu.USRUID
		 LEFT JOIN #FingerprintWarningUsers as fw WITH (NOLOCK) ON fw.UserId = bu.USRUID
		 LEFT JOIN #CardWarningUsers as cw WITH (NOLOCK) ON cw.UserId = bu.USRUID
	WHERE 
		e.[Enabled] = 1 AND e.[isDeleted] = 0
		AND e.IsSpecialUser = 0
		AND (
			(@EmployeeId IS NULL Or @EmployeeId <= 0)
			OR e.Id = @EmployeeId
			)
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )

	GROUP BY bu.USRUID 
		,e.Id 
		,e.Code 
		,e.FullName
		,e.NickName
		,e.GroupDate
		,c.CardNo
		,g.Code
		,g.[Rank]
		,fw.IsWarning
		,cw.IsWarning

	ORDER BY g.[Rank]
			,g.Code
			,e.GroupDate

	DROP TABLE #FingerprintSummary
	DROP TABLE #CardSummary
	DROP TABLE #CardWarningUsers
	DROP TABLE #FingerprintWarningUsers

END

GO
