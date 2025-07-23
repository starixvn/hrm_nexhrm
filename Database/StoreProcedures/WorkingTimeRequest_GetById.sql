CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetById] 
    @id BIGINT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by id.
DB: NexEPA
Original: 

Revisions:  
    - 20250707@Henry.Dinh: Query to bodb_HRM.dbo.Employee.
Param's Explanation:

*/
BEGIN
	DECLARE @IsAllowUpdateStatus TINYINT
	SET @IsAllowUpdateStatus = 1
    SELECT r.Id
        ,r.empID
        ,RequestType
        ,r.FromDate AS RequestedDate
        ,r.CreatedDate
        ,STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.NickName
        END AS ProcessedBy
        ,CASE
            WHEN r.ProcessedBy = -1 THEN 'System'
            ELSE ap.FullName
        END AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,ProcessedDate
        ,0.0 AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS TotalMins
        ,r.FromDate	
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,@IsAllowUpdateStatus as IsAllowUpdateStatus
		,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        LEFT JOIN bodb_HRM.dbo.Employee ap WITH (NOLOCK) ON ap.EmpId = r.ProcessedBy
        JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
            AND c.WorkDate = convert(VARCHAR(10), r.FromDate, 111)
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
    WHERE r.id = @id
END

GO
