ALTER PROCEDURE [dbo].[Employee_GetById] @EmployeeId INT
AS
/*
    Created: 20160205@Quinn.Vo
    Task: Get employee info by id
    DB: NexHRM

    Revisions:  
	    20191206@Merlin.Le: Add PlaceOfBirth, ContactName, UserRelationship, ContactPhone, ContactAddress to table Employee
	    20252602@Henry.Dinh: Add Management Grade
        20250530@Tim: add Salary Group
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATETIME
        ,@AnnualLeaveBalancePending NUMERIC(5, 1)
        ,@AnnualLeaveBalance NUMERIC(5, 1)
        ,@SubGroup VARCHAR(50)
		,@Approvers VARCHAR(255)

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

    SELECT @SubGroup = @SubGroup + COALESCE(CAST(eg.GroupId AS VARCHAR(10)) + ',', '')
    FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
    WHERE eg.EmpID = @EmployeeId
        AND ISNULL(isMainGroup, 0) = 0
		AND ISNULL(HandleRequest, 0) = 0

    SELECT @SubGroup = CASE 
            WHEN LEN(@SubGroup) > 0 THEN LEFT(@SubGroup, LEN(@SubGroup) - 1)
            ELSE ''
            END

    SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
		,[EducationBackground]
        ,[DOB]
        ,[Email]
        ,[Enabled]
        ,[Phone]
        ,[Gender]
        ,[Roles]
        ,[JoinedDate]
        ,[ConfirmedDate]
        ,[ResignedDate]
        ,[Skype]
        ,[Yahoo]
        ,[Marriage]
        ,ISNULL(@AnnualLeaveBalance, 0) - ISNULL(@AnnualLeaveBalancePending, 0) AS AnnualLeaveBalance
        ,@SubGroup AS SubGroups
		,gra.JobGradeId
		,gra.Code AS JobGradeCode
		,gra.[Name] AS JobGradeName
		,gra.[Description]
		,gra.AffectedDate
		,gra.Note
        ,mgra.ManagementGradeId
		,mgra.Code AS ManagementGradeCode
		,mgra.[Name] AS ManagementGradeName
		,mgra.[Description] AS ManagementDescription
		,mgra.AffectedDate AS ManagementAffectedDate
		,mgra.Note AS ManagementNote        
        ,e.SalaryGroupId
		,e.SocialInsuranceNo
        ,e.[CreatedDate]
        ,e.[CreatedBy]
        ,e.[ModifiedDate]
        ,e.[ModifiedBy]
        ,e.[IsDeleted]
		,[IsSpecialUser]
		,[GroupDate]
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
        ,ISNULL(sh2.[Name], 'Off') AS MonShiftName
        ,ISNULL([TueShift], 0) AS TueShiftId
        ,ISNULL(sh3.[Name], 'Off') AS TueShiftName
        ,ISNULL([WedShift], 0) AS WedShiftId
        ,ISNULL(sh4.[Name], 'Off') AS WedShiftName
        ,ISNULL([ThuShift], 0) AS ThuShiftId
        ,ISNULL(sh5.[Name], 'Off') AS ThuShiftName
        ,ISNULL([FriShift], 0) AS FriShiftId
        ,ISNULL(sh6.[Name], 'Off') AS FriShiftName
        ,ISNULL([SatShift], 0) AS SatShiftId
        ,ISNULL(sh7.[Name], 'Off') AS SatShiftName
        ,ISNULL([SunShift], 0) AS SunShiftId
        ,ISNULL(sh8.[Name], 'Off') AS SunShiftName
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
        ,g.[Name]
        ,ISNULL(e.JobTitleId, 0) AS JobTitleId
        ,jt.[Name]
    FROM dbo.Employee AS e WITH (NOLOCK)
        LEFT JOIN dbo.EmployeeShift AS es WITH (NOLOCK) ON e.Id = es.EmpID
        LEFT JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN HRM.dbo.TOEIC AS toeic WITH (NOLOCK) ON e.Id = toeic.EmpId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        LEFT JOIN dbo.[Shift] AS sh2 WITH (NOLOCK) ON es.MonShift = sh2.Id
        LEFT JOIN dbo.[Shift] AS sh3 WITH (NOLOCK) ON es.TueShift = sh3.Id
        LEFT JOIN dbo.[Shift] AS sh4 WITH (NOLOCK) ON es.WedShift = sh4.Id
        LEFT JOIN dbo.[Shift] AS sh5 WITH (NOLOCK) ON es.ThuShift = sh5.Id
        LEFT JOIN dbo.[Shift] AS sh6 WITH (NOLOCK) ON es.FriShift = sh6.Id
        LEFT JOIN dbo.[Shift] AS sh7 WITH (NOLOCK) ON es.SatShift = sh7.Id
        LEFT JOIN dbo.[Shift] AS sh8 WITH (NOLOCK) ON es.SunShift = sh8.Id
		LEFT JOIN (
			SELECT ejg.EmpId
			    ,ejg.AffectedDate
			    ,ejg.Note
			    ,jg.JobGradeId
			    ,jg.Code
			    ,jg.[Name]
			    ,jg.[Description]
			FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			    INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			WHERE ejg.EmpId = @EmployeeId 
			    AND ejg.IsCurrentUsed = 1
			    AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
		LEFT JOIN (
			SELECT emg.EmpId
			    ,emg.AffectedDate
			    ,emg.Note
			    ,mg.ManagementGradeId
			    ,mg.Code
			    ,mg.[Name]
			    ,mg.[Description]
			FROM HRM.dbo.EmployeeManagementGrade as emg  WITH (NOLOCK)
			    INNER JOIN HRM.dbo.ManagementGrade as mg WITH (NOLOCK) on emg.ManagementGradeId = mg.ManagementGradeId
			WHERE emg.EmpId = @EmployeeId 
			    AND emg.IsCurrentUsed = 1
			    AND ISNULL(emg.IsDeleted,0) = 0
		) AS mgra ON e.Id = mgra.EmpId

    WHERE e.Id = @EmployeeId
END