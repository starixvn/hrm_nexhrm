CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestHistory]
    @requestid bigint
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by all types.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @IsAllowUpdateStatus TINYINT
	SET @IsAllowUpdateStatus = 0
    SELECT
        r.Id,
        r.empID,
        RequestType,
        (CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s.Code END
            ) AS FromShift,
        s2.Code as ToShift,
        r.FromDate AS RequestedDate,
        r.FromDate,
        r.ToDate,
        r.TotalDays,
        lv.Id AS LeaveTypeId,
        lv.NAME AS LeaveTypeName,
        case when RequestType = 3 then LateInMins else EarlyOutMins end as TotalMins,    
        Remark,
        r.CreatedDate,
        Status,
        e.NickName AS Requester,
        e.FullName AS FullNameRequester,
        '' AS EmailRequester,
        g.Name AS Team,
        g.Code AS TeamCode,
        ap.NickName AS ProcessedBy,
        ap.FullName AS FullNameProcessedBy,
        '' AS EmailProcessedBy,
        ProcessedDate,
        0.0 AS TotalDaysRequestInMonth,
        @IsAllowUpdateStatus as IsAllowUpdateStatus,
		r.IsCrossDate
    FROM dbo.WorkingTimeRequest_history r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),r.FromDate,111)
        LEFT JOIN Shift s WITH (NOLOCK) ON s.id = c.ShiftID
        LEFT JOIN Employee ap WITH (NOLOCK) 
        ON (
        (ap.ID = r.ProcessedBy AND r.ProcessedBy IS NOT NULL)
        OR (ap.ID = r.CreatedBy AND r.ProcessedBy IS NULL)
        )
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id= r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id= r.ToShift
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
    WHERE r.Id = @requestid
    ORDER BY keyid
END

GO
