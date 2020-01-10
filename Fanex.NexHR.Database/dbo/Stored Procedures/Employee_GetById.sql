
CREATE PROCEDURE [dbo].[Employee_GetById] @EmployeeId INT
AS
/*
Created: 20160205@Quinn.Vo
Task: Get employee info by id
DB: NexHRM
Original: 

Revisions:  
	20191206@Merlin.Le: Add PlaceOfBirth, ContactName, UserRelationship, ContactPhone, ContactAddress to table Employee
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATETIME
        ,@AnnualLeaveBalancePending NUMERIC(5, 1)
        ,@AnnualLeaveBalance NUMERIC(5, 1)
        ,@SubGroup VARCHAR(50)
        ,@LeaderGroup VARCHAR(50)

    SET @Today = GETDATE()

    SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
    FROM dbo.LeaveTrans WITH (NOLOCK)
    WHERE EmpId = @EmployeeId
        AND ISNULL(Deleted, 0) = 0
        AND YEAR(FromDate) = YEAR(@Today)

    SELECT @AnnualLeaveBalancePending = SUM(ISNULL(wk.TotalDays, 0))
    FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE e.Id = @EmployeeId
        AND wk.RequestType = 6
        AND wk.[Status] = 1
        AND wk.LeaveTypeID IN (2, 30)
    GROUP BY e.Id

    SELECT @SubGroup = ''
        ,@LeaderGroup = ''

    SELECT @SubGroup = @SubGroup + COALESCE(CAST(eg.GroupId AS VARCHAR(10)) + ', ', '')
    FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
    WHERE eg.EmpID = @EmployeeId
        AND ISNULL(isMainGroup, 0) = 0
		AND ISNULL(HandleRequest, 0) = 0

    SELECT @SubGroup = CASE 
            WHEN LEN(@SubGroup) > 0 THEN LEFT(@SubGroup, LEN(@SubGroup) - 1)
            ELSE ''
            END

    SELECT @LeaderGroup = @LeaderGroup + COALESCE(CAST(eg.GroupId AS VARCHAR(10)) + ', ', '')
    FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
    WHERE eg.EmpID = @EmployeeId
        AND ISNULL(HandleRequest, 0) = 1

    SELECT @LeaderGroup = CASE 
            WHEN LEN(@LeaderGroup) > 0 THEN LEFT(@LeaderGroup, LEN(@LeaderGroup) - 1)
            ELSE ''
            END

    SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
        ,[DOB]
        ,[Email]
        ,[Enabled]
        ,[Phone]
        ,[Gender]
        ,[Roles]
        ,[ManagerId]
        ,[JoinedDate]
        ,[ConfirmedDate]
        ,[ResignedDate]
        ,[Skype]
        ,[Yahoo]
        ,[Mariage]
        ,[LastLogin]
        ,ISNULL(@AnnualLeaveBalance, 0) - ISNULL(@AnnualLeaveBalancePending, 0) AS AnnualLeaveBalance
        ,@SubGroup AS SubGroups
        ,@LeaderGroup AS LeaderGroups
        ,e.[CreatedDate]
        ,e.[CreatedBy]
        ,e.[ModifiedDate]
        ,e.[ModifiedBy]
        ,[isDeleted]
		,[IsSpecialUser]
		,[CompanyDate]
        ,[PermanentAddress] AS [Address]
        ,[CurrentAddress]
        ,[IdCardNumber]
        ,[PassportNo]
        ,[PassportExp]
        ,[GeoMapLatitude]
        ,[GeoMapLongtitude]
        ,[DistanceToCompany]
        ,[EmployeeCard]
        ,[IssuedDate]
        ,[IssuedPlace]
		,[PlaceOfBirth]
		,[ContactName]
		,[UserRelationship]
		,[ContactPhone]
		,[ContactAddress]
        ,ISNULL([MonShift], 0) AS MonShiftId
        ,ISNULL(sh2.NAME, 'Off') AS MonShiftName
        ,ISNULL([TueShift], 0) AS TueShiftId
        ,ISNULL(sh3.NAME, 'Off') AS TueShiftName
        ,ISNULL([WedShift], 0) AS WedShiftId
        ,ISNULL(sh4.NAME, 'Off') AS WedShiftName
        ,ISNULL([ThuShift], 0) AS ThuShiftId
        ,ISNULL(sh5.NAME, 'Off') AS ThuShiftName
        ,ISNULL([FriShift], 0) AS FriShiftId
        ,ISNULL(sh6.NAME, 'Off') AS FriShiftName
        ,ISNULL([SatShift], 0) AS SatShiftId
        ,ISNULL(sh7.NAME, 'Off') AS SatShiftName
        ,ISNULL([SunShift], 0) AS SunShiftId
        ,ISNULL(sh8.NAME, 'Off') AS SunShiftName
        ,LRExamDate
        ,Listening
        ,OldListening
        ,Reading
        ,OldReading
        ,SWExamDate
        ,Speaking
        ,OldSpeaking
        ,Writing
        ,OldWriting
        ,InputDate
        ,ISNULL(GroupId, 0) AS GroupId
        ,g.Code
        ,g.NAME
        ,ISNULL(e.JobTitleId, 0) AS JobTitleId
        ,lk.Code
        ,lk.NAME
    FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeShift AS es WITH (NOLOCK) ON e.Id = es.EmpID
        LEFT JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN dbo.TOEIC AS toeic WITH (NOLOCK) ON e.Id = toeic.EmpId
        LEFT JOIN dbo.LookupData AS lk WITH (NOLOCK) ON e.JobTitleId = lk.Id
        LEFT JOIN dbo.[Shift] AS sh2 WITH (NOLOCK) ON es.MonShift = sh2.Id
        LEFT JOIN dbo.[Shift] AS sh3 WITH (NOLOCK) ON es.TueShift = sh3.Id
        LEFT JOIN dbo.[Shift] AS sh4 WITH (NOLOCK) ON es.WedShift = sh4.Id
        LEFT JOIN dbo.[Shift] AS sh5 WITH (NOLOCK) ON es.ThuShift = sh5.Id
        LEFT JOIN dbo.[Shift] AS sh6 WITH (NOLOCK) ON es.FriShift = sh6.Id
        LEFT JOIN dbo.[Shift] AS sh7 WITH (NOLOCK) ON es.SatShift = sh7.Id
        LEFT JOIN dbo.[Shift] AS sh8 WITH (NOLOCK) ON es.SunShift = sh8.Id
    WHERE e.Id = @EmployeeId
END