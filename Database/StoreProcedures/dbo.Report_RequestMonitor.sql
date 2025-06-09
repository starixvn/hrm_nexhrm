USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Report_RequestMonitor]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/*
--Report_RequestMonitor '2015/8/12', '2015/8/12',1, 3, 125
*/
CREATE OR ALTER PROCEDURE [dbo].[Report_RequestMonitor] @fromDate DATETIME
    ,@toDate DATETIME
    ,@requestType INT = 0
    ,@maxRequest INT
    ,@role INT
    ,@employeeID INT
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
    DECLARE @CurrentDate DATETIME

    SET @CurrentDate = dbo.fnc_GetClientDateTime()

    IF OBJECT_ID('#Employee') IS NOT NULL
    BEGIN
        DROP TABLE #Employee
    END

    CREATE TABLE #Employee (EmpId INT)

    IF OBJECT_ID('#CheckInOutRequest') IS NOT NULL
    BEGIN
        DROP TABLE #CheckInOutRequest
    END

    CREATE TABLE #CheckInOutRequest (
        EmpId INT
        ,TotalRequest INT
        )

    IF OBJECT_ID('#LeaveRequestTimes') IS NOT NULL
    BEGIN
        DROP TABLE #LeaveRequestTimes
    END

    CREATE TABLE #LeaveRequestTimes (
        EmpId INT
        ,TotalRequest INT
        )

    IF OBJECT_ID('#LeaveRequestDays') IS NOT NULL
    BEGIN
        DROP TABLE #LeaveRequestDays
    END

    CREATE TABLE #LeaveRequestDays (
        EmpId INT
        ,TotalAnnualLeave NUMERIC(5, 1)
        ,TotalUnpaidLeave NUMERIC(5, 1)
        ,TotalOtherLeave NUMERIC(5, 1)
        )

    IF OBJECT_ID('#LateInEarlyOutAndAbsent') IS NOT NULL
    BEGIN
        DROP TABLE #LateInEarlyOutAndAbsent
    END

    CREATE TABLE #LateInEarlyOutAndAbsent (
        EmpId INT
        ,LateInEarlyOutTimes INT
        ,TotalAbsent INT
        ,TotalMissingCheckInOut INT
        )

    IF (
            @role = 3
            OR @role = 4
            )
    BEGIN
        INSERT INTO #Employee (EmpId)
        SELECT Id
        FROM dbo.Employee WITH (NOLOCK)
        WHERE ISNULL(isDeleted, 0) = 0
    END
    ELSE
    BEGIN
        INSERT INTO #Employee (EmpId)
		SELECT DISTINCT sup.EmpId
		FROM dbo.EmployeeSupDetail AS sup WITH (NOLOCK)
		WHERE sup.ApproverId = @employeeID
    END

    INSERT INTO #CheckInOutRequest (
        EmpId
        ,TotalRequest
        )
    SELECT wk.EmpId
        ,COUNT(1)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN #Employee AS e ON wk.EmpId = e.EmpId
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
        AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
    WHERE wk.RequestType IN (1, 2)
        AND wk.[Status] IN (1, 2)
        AND DATEDIFF(D, @fromdate, wk.FromDate) > = 0
        AND DATEDIFF(D, wk.FromDate, @toDate) > = 0
        AND cl.IsHoliday <> 1
		AND cl.IsOffDay <> 1 -- l? or ngày cu?i tu?n
    GROUP BY wk.EmpId

    INSERT INTO #LeaveRequestTimes (
        EmpId
        ,TotalRequest
        )
    SELECT wk.EmpId
        ,COUNT(1)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN #Employee AS e ON wk.EmpId = e.EmpId
    WHERE wk.RequestType = 6
        AND wk.[Status] IN (1, 2)
        AND (
            (
                DATEDIFF(D, @fromdate, wk.FromDate) > = 0
                AND DATEDIFF(D, wk.FromDate, @toDate) > = 0
                )
            OR (
                DATEDIFF(D, @fromdate, wk.ToDate) > = 0
                AND DATEDIFF(D, wk.ToDate, @toDate) > = 0
                )
            )
    GROUP BY wk.EmpId

    INSERT INTO #LeaveRequestDays (
        EmpId
        ,TotalAnnualLeave
        ,TotalUnpaidLeave
        ,TotalOtherLeave
        )
    SELECT wk.EmpId
        ,ISNULL(SUM(CASE 
                    WHEN (wk.LeaveTypeID IN (2, 30)) THEN (CASE 
                            WHEN wk.TotalDays = 0.5 THEN 0.5
                            ELSE 1 END
                            )
                    ELSE 0
                    END), 0)
        ,ISNULL(SUM(CASE 
                    WHEN (wk.LeaveTypeID = 26) THEN (CASE 
                            WHEN wk.TotalDays = 0.5 THEN 0.5
                            ELSE 1 END
                            )
                    ELSE 0
                    END), 0)
        ,ISNULL(SUM(CASE 
                    WHEN (wk.LeaveTypeID NOT IN (2, 26, 30)) THEN (CASE 
                            WHEN wk.TotalDays = 0.5 THEN 0.5
                            ELSE 1 END
                            )
                    ELSE 0
                    END), 0)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN #Employee AS e ON wk.EmpId = e.EmpId
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON DATEDIFF(D, wk.FromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, wk.ToDate) >= 0
        AND wk.EmpId = cl.EmpId
        AND cl.IsOffDay <> 1
        AND cl.IsHoliday <> 1
    WHERE wk.RequestType = 6
        AND wk.[Status] IN (1, 2)
        AND DATEDIFF(D, @fromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @toDate) >= 0
    GROUP BY wk.EmpId

    INSERT INTO #LateInEarlyOutAndAbsent (
        EmpId
        ,LateInEarlyOutTimes
        ,TotalAbsent
        ,TotalMissingCheckInOut
        )
    SELECT att.EmpId
        ,SUM((CASE 
                WHEN att.LateIn > 0 THEN 1
                ELSE 0 END
                ) + (CASE 
                WHEN att.EarlyOut > 0 THEN 1
                ELSE 0 END
                ))
        ,SUM(CASE 
                WHEN (
                        ISNULL(att.Absence, 0) = 1
                        AND cl.IsOffDay = 0
                        AND cl.IsHoliday = 0
                        AND ISNULL(att.LeaveDay, 0) <> 1
                        AND DATEDIFF(D, (att.AttendanceDate + ' ' + s.EndCheckInTime), @CurrentDate) > 0
                        ) THEN 1
                ELSE 0
                END)
        ,SUM((CASE 
                WHEN (
                        ISNULL(Absence, 0) = 0
                        AND att.InFixed IS NULL
                        AND DATEDIFF(D, (att.AttendanceDate + ' ' + s.EndCheckInTime), @CurrentDate) > 0
                        ) THEN 1
                ELSE 0 END
                ) + (CASE 
                WHEN (
                        ISNULL(Absence, 0) = 0
                        AND att.OutFixed IS NULL
                        AND DATEDIFF(D, (att.AttendanceDate + ' ' + s.EndCheckInTime), @CurrentDate) > 0
                        ) THEN 1
                ELSE 0 END
                ))
    FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
        INNER JOIN #Employee AS e ON att.EmpId = e.EmpId
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.EmpId = att.EmpId
        AND cl.Workdate = att.AttendanceDate
        LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Id = att.ShiftId
    WHERE DATEDIFF(D, @fromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @toDate) >= 0
    GROUP BY att.EmpId

    SELECT ISNULL(chk.TotalRequest, 0) AS TotalCheckInOutTimes
        ,ISNULL(lti.TotalRequest, 0) AS TotalLeaveTimes
        ,ISNULL(lda.TotalAnnualLeave, 0) AS TotalAnnualLeave
        ,ISNULL(lda.TotalUnpaidLeave, 0) AS TotalUnpaidLeave
        ,ISNULL(lda.TotalOtherLeave, 0) AS TotalOtherLeave
        ,ISNULL(li.LateInEarlyOutTimes, 0) AS LateInEarlyOutTimes
        ,ISNULL(li.TotalAbsent, 0) AS TotalAbsentTimes
        ,ISNULL(li.TotalMissingCheckInOut, 0) AS TotalMissingCheckInOutTimes
        ,ee.EmpId
        ,e.FullName
        ,e.NickName
        ,g.Id AS GroupId
        ,g.Code
        ,g.NAME
    FROM #Employee AS ee
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = ee.EmpId
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON g.Id = e.GroupID
        LEFT JOIN #CheckInOutRequest AS chk ON e.Id = chk.EmpId
        LEFT JOIN #LeaveRequestTimes AS lti ON e.Id = lti.EmpId
        LEFT JOIN #LeaveRequestDays AS lda ON e.Id = lda.EmpId
        LEFT JOIN #LateInEarlyOutAndAbsent AS li ON e.Id = li.EmpId
    WHERE 
		ISNULL(e.IsSpecialUser, 0) = 0
		AND
		(
			( (@requestType = 1) AND (			lti.TotalRequest >= @maxRequest
            OR lda.TotalAnnualLeave >= @maxRequest
            OR lda.TotalUnpaidLeave >= @maxRequest
            OR lda.TotalOtherLeave >= @maxRequest ) )
            
            OR ( (@requestType = 2) AND (       chk.TotalRequest >= @maxRequest
            OR li.TotalMissingCheckInOut >= @maxRequest))
            
            OR ( (@requestType = 3) AND (li.LateInEarlyOutTimes >= @maxRequest))
            
            OR ( (@requestType = 4) AND (li.TotalAbsent >= @maxRequest))
            
            OR ( (@requestType = 0) AND (                         
				lti.TotalRequest >= @maxRequest
				OR lda.TotalAnnualLeave >= @maxRequest
				OR lda.TotalUnpaidLeave >= @maxRequest
				OR lda.TotalOtherLeave >= @maxRequest   
				OR chk.TotalRequest >= @maxRequest
				OR li.TotalMissingCheckInOut >= @maxRequest            
				OR li.LateInEarlyOutTimes >= @maxRequest            
				OR li.TotalAbsent >= @maxRequest
				))                              
            )
END

GO
