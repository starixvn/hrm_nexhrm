
/****** Object:  Stored Procedure dbo.Report_HRLeaderDashBoard    Script Date: 3/13/2016 3:21:42 PM ******/
/*
--[Report_HRLeaderDashBoard] 125
*/

CREATE PROCEDURE [dbo].[Report_HRLeaderDashBoard]
    @EmpId INT
AS
/*
Created: 2012/05/03@Dan
Task: show dashboard info for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

BEGIN

DECLARE @CurrentMonth DATETIME
        ,@LastMonth DATETIME
        ,@Today DATETIME
        ,@Yesterday DATETIME
        ,@LeavePending INT
        ,@CheckInOutPending INT
        ,@LeaveApprovedCurrentMonth INT
        ,@LeaveApprovedLastMonth INT
        ,@LeaveRejectedCurrentMonth INT
        ,@LeaveRejectedLastMonth INT
        ,@CheckInOutApprovedCurrentMonth INT
        ,@CheckInOutApprovedLastMonth INT
        ,@CheckInOutRejectedCurrentMonth INT
        ,@CheckInOutRejectedLastMonth INT
        ,@FromDateLeave DATETIME
        ,@ToDateLeave DATETIME
        ,@FromDateCheckInOut DATETIME
        ,@ToDateCheckInOut DATETIME
        ,@IsManagement BIT
        ,@IsLeaderOfStandByTeam BIT

    IF (EXISTS (SELECT 1 
    FROM [dbo].[Employee] WITH (NOLOCK)
    WHERE ID = @EmpId AND GroupId = 11))
    BEGIN
        SET @IsManagement = 1
    END
    ELSE
    BEGIN
        SET @IsManagement = 0
    END

    IF (@EmpId IN (123, 138))
    BEGIN
        SET @IsLeaderOfStandByTeam = 1
    END
    ELSE
    BEGIN
        SET @IsLeaderOfStandByTeam = 0
    END

SET @Today = GETDATE()
    SELECT @CurrentMonth = @Today
        ,@LastMonth = DATEADD(M, - 1, @Today)
        ,@Yesterday = DATEADD(D, - 1, @Today)


IF (@IsLeaderOfStandByTeam = 1)
BEGIN
    SELECT @CheckInOutPending = COUNT(1)
        ,@FromDateCheckInOut = MIN (wk.FromDate)
        ,@ToDateCheckInOut = MAX (wk.FromDate)
    FROM [dbo].[WorkingTimeRequest] AS wk WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
        AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
    WHERE wk.RequestType IN (1, 2)
        AND wk.[Status] = 1
        AND (wk.EmpId <> @EmpId -- not count the leader itself 
                AND (wk.EmpId IN (SELECT EmpId
                        FROM  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                        WHERE eg.GroupId IN (SELECT Id
                                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                                            WHERE LeaderId = @EmpId OR    
                                                    AssistantLeaderId = @EmpId
                                        )
                            AND ISNULL(HandleRequest,0) = 1
                            )
                        OR (
                                cl.ShiftId = 14 AND DATEPART(DW, cl.WorkDate) IN (1, 7)
                            )
                        )
                )

    SELECT @CheckInOutApprovedCurrentMonth = SUM(CASE WHEN (wk.[Status] = 2 AND DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0) THEN 1 ELSE 0 END)
        ,@CheckInOutApprovedLastMonth = SUM(CASE WHEN (wk.[Status] = 2 AND DATEDIFF(M, wk.FromDate, @LastMonth) = 0) THEN 1 ELSE 0 END)
        ,@CheckInOutRejectedCurrentMonth = SUM(CASE WHEN (wk.[Status] = 3 AND DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0) THEN 1 ELSE 0 END)
        ,@CheckInOutRejectedLastMonth = SUM(CASE WHEN (wk.[Status] = 3 AND DATEDIFF(M, wk.FromDate, @LastMonth) = 0) THEN 1 ELSE 0 END)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
        AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
    WHERE wk.RequestType IN (1, 2)
        AND wk.[Status] IN (2, 3)
        AND DATEDIFF(M, @LastMonth, wk.FromDate) > = 0
        AND DATEDIFF(M, wk.FromDate, @CurrentMonth) > = 0
        AND (wk.EmpId <> @EmpId -- not count the leader itself 
                AND (wk.EmpId IN (SELECT EmpId
                        FROM  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                        WHERE eg.GroupId IN (SELECT Id
                                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                                            WHERE LeaderId = @EmpId OR    
                                                    AssistantLeaderId = @EmpId
                                        )
                            AND ISNULL(HandleRequest,0) = 1
                            )
                        OR (
                                cl.ShiftId = 14 AND DATEPART(DW, cl.WorkDate) IN (1, 7)
                            )
                        )
                )
END
ELSE
BEGIN
    SELECT @CheckInOutPending = COUNT(1)
        ,@FromDateCheckInOut = MIN (wk.FromDate)
        ,@ToDateCheckInOut = MAX (wk.FromDate)
    FROM [dbo].[WorkingTimeRequest] AS wk WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
        AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
    WHERE wk.RequestType IN (1, 2)
        AND wk.[Status] = 1
        AND (wk.EmpId <> @EmpId -- not count the leader itself 
                AND (wk.EmpId IN (SELECT EmpId
                        FROM  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                        WHERE eg.GroupId IN (SELECT Id
                                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                                            WHERE LeaderId = @EmpId OR    
                                                    AssistantLeaderId = @EmpId
                                        )
                            AND ISNULL(HandleRequest,0) = 1
                            )
                        AND (
                                cl.ShiftId <> 14 OR DATEPART(DW, cl.WorkDate) NOT IN (1, 7)
                            )
                        )
                )

    SELECT @CheckInOutApprovedCurrentMonth = SUM(CASE WHEN (wk.[Status] = 2 AND DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0) THEN 1 ELSE 0 END)
        ,@CheckInOutApprovedLastMonth = SUM(CASE WHEN (wk.[Status] = 2 AND DATEDIFF(M, wk.FromDate, @LastMonth) = 0) THEN 1 ELSE 0 END)
        ,@CheckInOutRejectedCurrentMonth = SUM(CASE WHEN (wk.[Status] = 3 AND DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0) THEN 1 ELSE 0 END)
        ,@CheckInOutRejectedLastMonth = SUM(CASE WHEN (wk.[Status] = 3 AND DATEDIFF(M, wk.FromDate, @LastMonth) = 0) THEN 1 ELSE 0 END)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
        AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
    WHERE wk.RequestType IN (1, 2)
        AND wk.[Status] IN (2, 3)
        AND DATEDIFF(M, @LastMonth, wk.FromDate) > = 0
        AND DATEDIFF(M, wk.FromDate, @CurrentMonth) > = 0
        AND (wk.EmpId <> @EmpId -- not count the leader itself 
                AND (wk.EmpId IN (SELECT EmpId
                        FROM  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                        WHERE eg.GroupId IN (SELECT Id
                                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                                            WHERE LeaderId = @EmpId OR    
                                                    AssistantLeaderId = @EmpId
                                        )
                            AND ISNULL(HandleRequest,0) = 1
                            )
                        AND (
                                cl.ShiftId <> 14 OR DATEPART(DW, cl.WorkDate) NOT IN (1, 7)
                            )
                        )
                )
END

IF (@IsManagement = 1)
BEGIN
    SELECT @LeavePending = COUNT(1)
        ,@FromDateLeave = MIN (wk.FromDate)
        ,@ToDateLeave = MAX (wk.FromDate)
    FROM [dbo].[WorkingTimeRequest] AS wk WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE wk.RequestType = 6
        AND wk.[Status] = 1
        AND (wk.EmpId <> @EmpId -- not count the leader itself 
                AND (wk.EmpId IN (SELECT EmpId
                        FROM  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                        WHERE eg.GroupId IN (SELECT Id
                                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                                            WHERE LeaderId = @EmpId OR
                                                    AssistantLeaderId = @EmpId
                                        )
                            AND ISNULL(HandleRequest,0) = 1
                            )
                        OR (
                             wk.TotalDays >= 5 AND wk.LeaveTypeID IN (2, 30)
                            )
                        OR wk.LeaveTypeID = 26
                        )
                )

    SELECT @LeaveApprovedCurrentMonth = SUM(CASE WHEN (wk.[Status] = 2 AND DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0) THEN 1 ELSE 0 END)
        ,@LeaveApprovedLastMonth = SUM(CASE WHEN (wk.[Status] = 2 AND DATEDIFF(M, wk.FromDate, @LastMonth) = 0) THEN 1 ELSE 0 END)
        ,@LeaveRejectedCurrentMonth = SUM(CASE WHEN (wk.[Status] = 3 AND DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0) THEN 1 ELSE 0 END)
        ,@LeaveRejectedLastMonth = SUM(CASE WHEN (wk.[Status] = 3 AND DATEDIFF(M, wk.FromDate, @LastMonth) = 0) THEN 1 ELSE 0 END)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE wk.RequestType = 6
        AND wk.[Status] IN (2, 3)
        AND ((DATEDIFF(M, @LastMonth, wk.FromDate) > = 0
                AND DATEDIFF(M, wk.FromDate, @CurrentMonth) > = 0)
            OR (
                DATEDIFF(M, @LastMonth, wk.ToDate) > = 0
                AND DATEDIFF(M, wk.ToDate, @CurrentMonth) > = 0
                )
            )
        AND (wk.EmpId <> @EmpId -- not count the leader itself 
                AND (wk.EmpId IN (SELECT EmpId
                        FROM  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                        WHERE eg.GroupId IN (SELECT Id
                                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                                            WHERE LeaderId = @EmpId OR    
                                                    AssistantLeaderId = @EmpId
                                        )
                            AND ISNULL(HandleRequest,0) = 1
                            )
                        OR (
                                wk.TotalDays >= 5 AND wk.LeaveTypeID IN (2, 30)
                            )
                        OR wk.LeaveTypeID = 26
                        )
                )
END
ELSE
BEGIN
    SELECT @LeavePending = COUNT(1)
        ,@FromDateLeave = MIN (wk.FromDate)
        ,@ToDateLeave = MAX (wk.FromDate)
    FROM [dbo].[WorkingTimeRequest] AS wk WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE wk.RequestType = 6
        AND wk.[Status] = 1
        AND (wk.EmpId <> @EmpId -- not count the leader itself 
                AND (wk.EmpId IN (SELECT EmpId
                        FROM  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                        WHERE eg.GroupId IN (SELECT Id
                                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                                            WHERE LeaderId = @EmpId OR    
                                                    AssistantLeaderId = @EmpId
                                        )
                            AND ISNULL(HandleRequest,0) = 1
                            )
                        AND (
                                wk.TotalDays < 5 OR wk.LeaveTypeID NOT IN (2, 30)
                            )
                        AND wk.LeaveTypeID <> 26
                        )
                )

    SELECT @LeaveApprovedCurrentMonth = SUM(CASE WHEN (wk.[Status] = 2 AND DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0) THEN 1 ELSE 0 END)
        ,@LeaveApprovedLastMonth = SUM(CASE WHEN (wk.[Status] = 2 AND DATEDIFF(M, wk.FromDate, @LastMonth) = 0) THEN 1 ELSE 0 END)
        ,@LeaveRejectedCurrentMonth = SUM(CASE WHEN (wk.[Status] = 3 AND DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0) THEN 1 ELSE 0 END)
        ,@LeaveRejectedLastMonth = SUM(CASE WHEN (wk.[Status] = 3 AND DATEDIFF(M, wk.FromDate, @LastMonth) = 0) THEN 1 ELSE 0 END)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE wk.RequestType = 6
        AND wk.[Status] IN (2, 3)
        AND ((DATEDIFF(M, @LastMonth, wk.FromDate) > = 0
                AND DATEDIFF(M, wk.FromDate, @CurrentMonth) > = 0)
            OR (
                DATEDIFF(M, @LastMonth, wk.ToDate) > = 0
                AND DATEDIFF(M, wk.ToDate, @CurrentMonth) > = 0
                )
            )
        AND (wk.EmpId <> @EmpId -- not count the leader itself 
                AND (wk.EmpId IN (SELECT EmpId
                        FROM  dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                        WHERE eg.GroupId IN (SELECT Id
                                            FROM dbo.EmployeeGroup WITH (NOLOCK)
                                            WHERE LeaderId = @EmpId OR    
                                                    AssistantLeaderId = @EmpId
                                        )
                            AND ISNULL(HandleRequest,0) = 1
                            )
                        AND (
                                wk.TotalDays < 5 OR wk.LeaveTypeID NOT IN (2, 30)
                            )
                        AND wk.LeaveTypeID <> 26
                        )
                )
END

SELECT ISNULL(@LeavePending, 0) AS LeavePending
    ,ISNULL(@CheckInOutPending, 0) AS CheckInOutPending
    ,ISNULL(@FromDateLeave, @Today) AS FromDateLeave
    ,ISNULL(@ToDateLeave, @Today) AS ToDateLeave
    ,ISNULL(@FromDateCheckInOut, @Today) AS FromDateCheckInOut
    ,ISNULL(@ToDateCheckInOut, @Today) AS ToDateCheckInOut
    ,ISNULL(@LeaveApprovedCurrentMonth, 0) AS LeaveApprovedCurrentMonth
    ,ISNULL(@LeaveApprovedLastMonth, 0) AS LeaveApprovedLastMonth
    ,ISNULL(@LeaveRejectedCurrentMonth, 0) AS LeaveRejectedCurrentMonth
    ,ISNULL(@LeaveRejectedLastMonth, 0) AS LeaveRejectedLastMonth
    ,ISNULL(@CheckInOutApprovedCurrentMonth, 0) AS CheckInOutApprovedCurrentMonth
    ,ISNULL(@CheckInOutApprovedLastMonth, 0) AS CheckInOutApprovedLastMonth
    ,ISNULL(@CheckInOutRejectedCurrentMonth, 0) AS CheckInOutRejectedCurrentMonth
    ,ISNULL(@CheckInOutRejectedLastMonth, 0) AS CheckInOutRejectedLastMonth

END
