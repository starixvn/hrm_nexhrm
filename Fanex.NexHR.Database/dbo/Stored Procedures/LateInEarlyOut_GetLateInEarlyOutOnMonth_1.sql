CREATE PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutOnMonth] 
	@Year INT
	,@Month INT
	
AS
/*
Created: 20200708@Merlin.Le
Task: Get late in/ early out on current month
DB: NexHRM & NexBot
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

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
	WHERE e.GroupId NOT IN (11,40)
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