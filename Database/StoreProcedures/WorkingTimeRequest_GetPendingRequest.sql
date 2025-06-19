CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetPendingRequest] 
AS
/*
Created: 20240621@Oliver.Dang
Task: Get all pending requests.
DB: NexHRM
Original: 

Revisions:    
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
	DECLARE @LeaderOfStandbyTeam INT,
		@StandByTeam VARCHAR(50)
		,@CoreTeamId INT;

	SELECT @CoreTeamId = MAX(CASE WHEN ss.Name = 'CoreTeamCode' THEN gs.GroupId END)
	FROM bodb_HRM.dbo.GroupSetting as gs WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name = 'CoreTeamCode'
	WHERE gs.OldGroupCode = ss.Value

	SELECT @LeaderOfStandbyTeam = e.Id FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON e.Id = st.Value AND st.Name = 'LeaderOfStandbyTeam'

	SELECT @StandByTeam = st.Value FROM dbo.SystemSetting AS st WITH (NOLOCK) WHERE st.Name = 'StanbyTeam'

    IF OBJECT_ID('tempdb..#PendingRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingRequest
	END

	IF OBJECT_ID('tempdb..#ManagementApprover') IS NOT NULL
	BEGIN
		DROP TABLE #ManagementApprover
	END

	CREATE TABLE #ManagementApprover(
		ApproverId INT
		,Approver NVARCHAR(20)
		,ApproverFullName NVARCHAR(50)
		,ApproverConversationId VARCHAR(200)
	)

	CREATE TABLE #PendingRequest(
		RequestId BIGINT
		,EmpId INT
		,GroupId INT
		,TeamName NVARCHAR(50)
		,Code VARCHAR(20)
		,RequestType SMALLINT
		,CreatedDate DATETIME
		,Requester NVARCHAR(20)
		,FullNameRequester NVARCHAR(50)
		,FromDate DATETIME
		,ToDate DATETIME
		,TotalDays NUMERIC(5, 1)
		,FromShift VARCHAR(10)
		,LeaveTypeId INT
		,LeaveTypeName NVARCHAR(100)
		,Remark NVARCHAR(2000)
		,IsManagementApprove BIT
		,IsStandbyAtWeekend BIT
		)

	INSERT INTO #PendingRequest
	SELECT r.Id as RequestId
		  ,r.EmpId
		  ,e.GroupId
		  ,g.Code
		  ,e.Code
		  ,r.RequestType
		  ,r.CreatedDate
  		  ,e.NickName AS Requester
		  ,e.FullName AS FullNameRequester
		  ,r.FromDate
		  ,r.ToDate
  		  ,r.TotalDays
		  ,s.Code
		  ,lv.Id AS LeaveTypeId
		  ,lv.[Name] AS LeaveTypeName
		  ,r.Remark
		  ,(CASE WHEN 
				(r.RequestType = 6) AND -- Leave type
					(r.LeaveTypeID IN (2, 30, 31) AND r.TotalDays >= 5) -- Annual Leave >= 5 days
					OR 
					r.LeaveTypeID = 26 -- Unpaid leave
			THEN 1
			ELSE 0
			END) as IsManagementApprove
			,(CASE WHEN 
				(r.RequestType IN (1,2) AND DATEPART(DW, r.FromDate) IN (1, 7) AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ',')))
			THEN 1
			ELSE 0
			END) as IsStandbyAtWeekend
	FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
	INNER JOIN dbo.Employee e with (nolock) on r.EmpId = e.Id
	INNER JOIN dbo.EmployeeGroup g with (nolock) on e.GroupId = g.Id
	LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
	LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
		AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
	LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.id = c.ShiftID
	WHERE e.[Enabled] = 1 AND ISNULL(e.isDeleted, 0 ) = 0 
		AND r.RequestType IN (1,2,6)
		AND r.[Status] = 1
	ORDER BY r.CreatedDate DESC


	IF EXISTS(SELECT 1
	FROM #PendingRequest 
	WHERE IsManagementApprove = 1)
	BEGIN
		INSERT INTO #ManagementApprover
		SELECT e.Id
			  ,e.NickName
			  ,e.FullName
			  ,con.ConversationId
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS eg ON e.GroupId = eg.Id
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
		WHERE e.[Enabled] = 1 AND eg.Id = @CoreTeamId AND con.ConversationId <> ''
	END

	SELECT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,ap.ApproverId
		 ,ap.Approver
		 ,ap.ApproverFullName
		 ,ap.ApproverConversationId
	FROM #PendingRequest pr
	CROSS JOIN  #ManagementApprover ap
	WHERE IsManagementApprove = 1

	UNION
	SELECT DISTINCT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,e.Id as ApproverId
		 ,e.NickName as Approver
		 ,e.FullName
		 ,con.ConversationId as ApproverConversationId
	FROM #PendingRequest pr
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = @LeaderOfStandbyTeam
	LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId AND con.ConversationId <> ''
	WHERE pr.IsStandbyAtWeekend = 1

	UNION

	SELECT DISTINCT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,sup.ApproverId as ApproverId
		 ,sup.ApproverNickName as Approver
		 ,sup.ApproverFullName
		 ,con.ConversationId as ApproverConversationId
	FROM #PendingRequest pr
	INNER JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON pr.EmpId = sup.EmpId
	LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON sup.ApproverId = con.EmpId AND con.ConversationId <> ''
	WHERE pr.IsManagementApprove = 0
		AND pr.IsStandbyAtWeekend = 0

	IF OBJECT_ID('tempdb..#PendingRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingRequest
	END

	IF OBJECT_ID('tempdb..#ManagementApprover') IS NOT NULL
	BEGIN
		DROP TABLE #ManagementApprover
	END
END
GO
