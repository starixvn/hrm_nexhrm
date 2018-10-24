

CREATE PROCEDURE [dbo].[Bio_GetFigerprintAndCardSummary]
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

	CREATE TABLE #FingerprintWarningUsers(UserId INT, IsWarning BIT)

	INSERT INTO #FingerprintWarningUsers(UserId, IsWarning)
	
	SELECT t.nUserIdn as UserId
			,1
	FROM [BioStar].dbo.TB_USER_TEMPLATE as t with (nolock) 
	LEFT JOIN [NexHRM].dbo.BioFingerprint as f with (nolock) 
		ON f.nUserIdn = t.nUserIdn AND f.nIndex = t.nIndex AND f.nTemplateIndex = t.nTemplateIndex
	WHERE f.EmpId IS NULL OR f.[Status] = 0
		OR t.bTemplate <> f.bTemplate
	GROUP BY t.nUserIdn

	CREATE TABLE #CardWarningUsers(UserId INT, IsWarning BIT)

	INSERT INTO #CardWarningUsers(UserId, IsWarning)
	
	SELECT DISTINCT c.nUserIdn, 1
	FROM [BioStar].dbo.TB_USER_CARD as c with (nolock) 
	LEFT JOIN [NexHRM].dbo.BioCard as bc with (nolock) 
		ON bc.nCardIdn = c.nCardIdn
	WHERE (bc.EmpId IS NULL OR bc.[Status] = 0) AND (c.sCardNo != '0' AND c.nLatest = 1 AND c.nStatus = 1)
	GROUP BY c.nUserIdn
	
	CREATE TABLE #FingerGroupByUserIdAndIndex
	(nUserId INT
	,nIndex SMALLINT)
	
	INSERT INTO #FingerGroupByUserIdAndIndex
	(nUserId, nIndex)
	SELECT 
		bt.nUserIdn
		,bt.nIndex
	FROM [BioStar].[dbo].[TB_USER_TEMPLATE] AS bt
	GROUP BY bt.nUserIdn, bt.nIndex
		
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
	SELECT 
		uc.nUserIdn
		,uc.sCardNo
	FROM [BioStar].[dbo].[TB_USER_CARD] AS uc WITH (NOLOCK)
	WHERE uc.sCardNo != '0' AND uc.nStatus = 1 AND uc.nLatest = 1

	SELECT bu.nUserIdn AS BioUserId
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
		,g.Id AS GroupId
		,g.Code
		,g.Name
		
	FROM 
		 dbo.Employee AS e WITH (NOLOCK)
		 JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		 JOIN [BioStar].[dbo].[TB_USER] AS bu WITH (NOLOCK) ON '0' + bu.sUserID = e.Code
		 LEFT JOIN #FingerprintSummary AS f WITH (NOLOCK) ON f.nUserId = bu.nUserIdn
		 LEFT JOIN #CardSummary AS c WITH (NOLOCK) ON c.nUserId = bu.nUserIdn
		 LEFT JOIN #FingerprintWarningUsers as fw WITH (NOLOCK) ON fw.UserId = bu.nUserIdn
		 LEFT JOIN #CardWarningUsers as cw WITH (NOLOCK) ON cw.UserId = bu.nUserIdn

	WHERE 
		e.[Enabled] = 1 AND e.[isDeleted] = 0
		AND (
			(@EmployeeId IS NULL Or @EmployeeId <= 0)
			OR e.Id = @EmployeeId
			)
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )

	GROUP BY bu.nUserIdn 
		,e.Id 
		,e.Code 
		,e.FullName
		,e.NickName
		,c.CardNo
		,g.Id
		,g.Code
		,g.Name
		,g.[Rank]
		,fw.IsWarning
		,cw.IsWarning

	ORDER BY g.[Rank]
			,g.Code
			,e.Code

	DROP TABLE #FingerprintSummary
	DROP TABLE #CardSummary
	DROP TABLE #CardWarningUsers
	DROP TABLE #FingerprintWarningUsers

END
