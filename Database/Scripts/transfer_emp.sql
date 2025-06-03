

DECLARE @EmployeeId INT = 86
        ,@ResignedDate DATE = '2025-06-01'
        ,@AffectedDate DATE = '2025-06-01'
        --,@ToGroupId INT = 1
        ,@ShiftId INT
        ,@IsTransferLateInEarlyOut BIT = 1
        ,@IsTransferAnnualLeaveBalance BIT = 1
        ,@IsTransferNexBalance BIT = 1
        ,@IsSpecialUser BIT = 0
        ,@FromDomain VARCHAR(20) = N'lumiplus\'
        ,@ToDomain VARCHAR(20) = ''
        ,@FromDomainEmail VARCHAR(20) = N'nexcel.com'
        ,@ToDomainEmail VARCHAR(20) = N'nexvision.vn'
        ,@ToCompanyId INT = 3
        ,@Todate DATETIME
        ,@NexBalance INT = 0
        --,@Password VARCHAR(100) = N'$2a$11$/xt4J0boH6u4CZJoUbz3b.jWxp1KQxcQXTZoFhKlSycLnkyI6QcCq'
                
DECLARE @CreditDays NUMERIC(18, 1)
        ,@LateInEarlyOut INT
        
-- INSERT INTO NexVisionHRM.dbo.EmployeeExt (Id, DomainUsername, Email, IsSpecialUser, IsDeleted
--         ,[Enabled], GroupID, Roles, JoinedDate, ConfirmedDate, Password)
-- SELECT @EmployeeId
--         ,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
--         ,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
--         ,@IsSpecialUser, 0, 1, @ToGroupId, Roles, @AffectedDate, @AffectedDate, @Password
-- FROM LumiHRM.dbo.EmployeeExt WITH (NOLOCK)
-- WHERE Id = @EmployeeId

UPDATE NexVisionHRM.dbo.EmployeeExt
SET JoinedDate = @AffectedDate, ConfirmedDate = @AffectedDate
WHERE Id = @EmployeeId

-- INSERT INTO NexVisionHRM.dbo.EmployeeGroup_Employee (GroupId, EmpID, isMainGroup)
-- VALUES (@ToGroupId, @EmployeeId, 1)

-- add shift
-- SELECT @ShiftId = MonShift
-- FROM LumiHRM.dbo.EmployeeShift WITH (NOLOCK)
-- WHERE EmpId = @EmployeeId

-- INSERT INTO NexVisionHRM.dbo.EmployeeShift ([EmpID], [MonShift], [TueShift], [WedShift], [ThuShift], [FriShift])
-- VALUES (@EmployeeId, @ShiftId, @ShiftId, @ShiftId, @ShiftId, @ShiftId)

-- add calendar
SET @todate = DATEADD(yy, DATEDIFF(yy, 0, GETDATE()) + 1, -1)

EXECUTE NexVisionHRM.dbo.EmployeeCalendar_Insert @EmployeeId
    ,@AffectedDate
    ,@todate

-- AL
IF @IsTransferAnnualLeaveBalance = 1
BEGIN
        SELECT @CreditDays = SUM(ISNULL (CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
        FROM LumiHRM.dbo.LeaveTrans WITH (NOLOCK)
        WHERE EmpId = @EmployeeId 
                AND YEAR(FromDate) = YEAR(@AffectedDate) 
                AND ISNULL(Deleted, 0) <> 1

        INSERT INTO NexVisionHRM.dbo.LeaveTrans (EmpId, Description, FromDate, ToDate, DeductDays, CreditDays, CreatedDate)
        VALUES (@EmployeeId, 'Init', @AffectedDate, @ToDate, 0, @CreditDays, GETDATE())
END

-- Late In/Early Out
IF @IsTransferLateInEarlyOut = 1
BEGIN
        SELECT @LateInEarlyOut = [Minutes]
        FROM LumiHRM.dbo.LateInEarlyOutYearly WITH (NOLOCK)
        WHERE EmpId = @EmployeeId AND [Year] = YEAR(@AffectedDate)

        IF @LateInEarlyOut IS NOT NULL
        BEGIN
                INSERT INTO NexVisionHRM.dbo.LateInEarlyOutTrans([EmpId], [WorkDate], [Type], [Minutes], [ModifiedDate], [IsManual])
                VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, GETDATE(), 1)
        END
                        
        INSERT INTO NexVisionHRM.dbo.LateInEarlyOutYearly([EmpId], [Year], [Minutes], [ModifiedDate])
        VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), GETDATE())
END

IF @IsTransferNexBalance = 1
BEGIN
        SELECT @NexBalance = CASE 
                        WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
                        ELSE 0 END
        FROM LumiHRM.dbo.Employee AS e WITH (NOLOCK)
                LEFT JOIN LumiHRM.dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId AND oty.[Year] = YEAR(@AffectedDate)
                LEFT JOIN LumiHRM.dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId AND la.[Year] = YEAR(@AffectedDate)
        WHERE e.Id = @EmployeeId

        IF @NexBalance <> 0
        BEGIN
                INSERT INTO NexVisionHRM.dbo.OvertimeTrans(EmpId, [Date], [Minutes], [Type], ModifiedDate)
                VALUES (@EmployeeId, @AffectedDate, @NexBalance, 1, GETDATE())
        END

        INSERT INTO NexVisionHRM.dbo.OvertimeYearly (EmpId, [Year], [Minutes], [ModifiedDate])
        VALUES (@EmployeeId, YEAR(@AffectedDate), @NexBalance, GETDATE())
END        

-- INSERT INTO HRM.dbo.EmployeeCompany(EmpId, CompanyId)
-- VALUES (@EmployeeId, @ToCompanyId)

/*
UPDATE LumiHRM.dbo.EmployeeExt WITH (ROWLOCK, UPDLOCK)
SET [Enabled] = 0
        ,ResignedDate = @ResignedDate
WHERE Id = @EmployeeId

UPDATE HRM.dbo.Employee WITH (ROWLOCK, UPDLOCK)
SET ModifiedDate = GETDATE()
WHERE Id = @EmployeeId
*/