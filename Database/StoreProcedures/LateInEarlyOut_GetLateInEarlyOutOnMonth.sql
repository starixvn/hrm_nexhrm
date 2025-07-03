CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutOnMonth] 
	@Year INT
	,@Month INT
	
AS
/*
	Created: 20200708@Merlin.Le
	Task: Get late in/ early out on current month
	DB: NexHRM & NexBot
	Original: 

	Revisions:  
		20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CoreTeamId NVARCHAR(50);
	DECLARE @ExpId NVARCHAR(50);
	DECLARE @CEOTeamId NVARCHAR(50);

	SELECT @ExpId = ISNULL(MAX(CASE WHEN ss.Name = 'EXPTeamId' THEN g.Id END), 0),
		@CoreTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CoreTeamId' THEN g.Id END), 0),
		@CEOTeamId = ISNULL(MAX(CASE WHEN ss.Name = 'CEOTeamId' THEN g.Id END), 0)
	FROM dbo.SystemSetting ss WITH (NOLOCK)
	INNER JOIN dbo.[EmployeeGroup] g WITH (NOLOCK) ON g.GroupTeamId = ss.Value
	WHERE ss.Name IN ('EXPTeamId', 'CoreTeamId', 'CEOTeamId')

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary
	END

	CREATE TABLE #LateInEarlyOutSummary(
		EmpId INT
		,MinuteOnMonth INT
		,TimeOnMonth INT)

	INSERT INTO #LateInEarlyOutSummary(EmpId, MinuteOnMonth, TimeOnMonth)

	SELECT EmpId
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0
						THEN 1
				ELSE 0
				END)
	FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE e.GroupId NOT IN (@ExpId, @CoreTeamId, @CEOTeamId)
	 AND YEAR(WorkDate) = @Year AND Month(WorkDate) = @Month
	GROUP BY EmpId

	SELECT e.Id AS EmpId
		  ,e.FullName
		  ,e.NickName
		  ,ec.ConversationId
		  ,ls.MinuteOnMonth
		  ,ls.TimeOnMonth
	FROM #LateInEarlyOutSummary AS ls WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON ls.EmpId = e.Id
		INNER JOIN [dbo].[EmployeeGroup] AS g WITH (NOLOCK) ON e.GroupId = g.Id
		--LEFT JOIN [NexBot].[dbo].[ConversationUser] AS c WITH (NOLOCK) ON e.NickName = c.NickName
		LEFT JOIN [dbo].[EmployeeConversation] AS ec WITH (NOLOCK) ON ec.EmpId = e.Id
	WHERE e.isDeleted = 0 AND e.[Enabled] = 1
		AND ls.TimeOnMonth >= 3
		
	DROP TABLE #LateInEarlyOutSummary	
END
GO
