DECLARE @EmployeeId INT = 4966
        ,@ResignedDate DATE
        ,@AffectedDate DATE = '2025-06-01'
        ,@ToGroupId INT = 3
        ,@ShiftId INT
        ,@IsTransferLateInEarlyOut BIT = 0
        ,@IsTransferAnnualLeaveBalance BIT = 0
        ,@IsTransferNexBalance BIT = 0
        ,@IsSpecialUser BIT = 1
        ,@FromDomain VARCHAR(20) = N'spvn\'
        ,@ToDomain VARCHAR(20) = N'lumiplus\'
        ,@FromDomainEmail VARCHAR(20) = N'nexcel.com'
        ,@ToDomainEmail VARCHAR(20) = N'lumiplus.vn'
        ,@ToCompanyId INT = 2
        ,@Todate DATETIME
        ,@NexBalance INT = 0
        ,@Roles VARCHAR(20) = '4'
                
DECLARE @CreditDays NUMERIC(18, 1)
        ,@LateInEarlyOut INT
        
INSERT INTO LumiHRM.dbo.EmployeeExt (Id, DomainUsername, Email, IsSpecialUser, IsDeleted
        ,[Enabled], GroupID, Roles, JoinedDate, ConfirmedDate)
SELECT @EmployeeId
        ,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
        ,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
        ,@IsSpecialUser, 0, 1, @ToGroupId, @Roles, @AffectedDate, @AffectedDate
FROM NexHRM.dbo.EmployeeExt WITH (NOLOCK)
WHERE Id = @EmployeeId

INSERT INTO LumiHRM.dbo.EmployeeGroup_Employee (GroupId, EmpID, isMainGroup)
VALUES (@ToGroupId, @EmployeeId, 1)

-- add shift
SELECT @ShiftId = MonShift
FROM NexHRM.dbo.EmployeeShift WITH (NOLOCK)
WHERE EmpId = @EmployeeId

INSERT INTO LumiHRM.dbo.EmployeeShift ([EmpID], [MonShift], [TueShift], [WedShift], [ThuShift], [FriShift])
VALUES (@EmployeeId, @ShiftId, @ShiftId, @ShiftId, @ShiftId, @ShiftId)

-- add calendar
SET @todate = DATEADD(yy, DATEDIFF(yy, 0, GETDATE()) + 1, -1)

EXECUTE LumiHRM.dbo.EmployeeCalendar_Insert @EmployeeId
    ,@AffectedDate
    ,@todate

-- AL
IF @IsTransferAnnualLeaveBalance = 1
BEGIN
        SELECT @CreditDays = SUM(ISNULL (CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
        FROM NexHRM.dbo.LeaveTrans WITH (NOLOCK)
        WHERE EmpId = @EmployeeId 
                AND YEAR(FromDate) = YEAR(@AffectedDate) 
                AND ISNULL(Deleted, 0) <> 1

        INSERT INTO LumiHRM.dbo.LeaveTrans (EmpId, Description, FromDate, ToDate, DeductDays, CreditDays, CreatedDate)
        VALUES (@EmployeeId, 'Init', @AffectedDate, @ToDate, 0, @CreditDays, GETDATE())
END

-- Late In/Early Out
IF @IsTransferLateInEarlyOut = 1
BEGIN
        SELECT @LateInEarlyOut = [Minutes]
        FROM NexHRM.dbo.LateInEarlyOutYearly WITH (NOLOCK)
        WHERE EmpId = @EmployeeId AND [Year] = YEAR(@AffectedDate)

        IF @LateInEarlyOut IS NOT NULL
        BEGIN
                INSERT INTO LumiHRM.dbo.LateInEarlyOutTrans([EmpId], [WorkDate], [Type], [Minutes], [ModifiedDate], [IsManual])
                VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, GETDATE(), 1)
        END
                        
        INSERT INTO LumiHRM.dbo.LateInEarlyOutYearly([EmpId], [Year], [Minutes], [ModifiedDate])
        VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), GETDATE())
END

IF @IsTransferNexBalance = 1
BEGIN
        SELECT @NexBalance = CASE 
                        WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
                        ELSE 0 END
        FROM NexHRM.dbo.Employee AS e WITH (NOLOCK)
                LEFT JOIN NexHRM.dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId AND oty.[Year] = YEAR(@AffectedDate)
                LEFT JOIN NexHRM.dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId AND la.[Year] = YEAR(@AffectedDate)
        WHERE e.Id = @EmployeeId

        IF @NexBalance <> 0
        BEGIN
                INSERT INTO LumiHRM.dbo.OvertimeTrans(EmpId, [Date], [Minutes], [Type], ModifiedDate)
                VALUES (@EmployeeId, @AffectedDate, @NexBalance, 1, GETDATE())
        END

        INSERT INTO LumiHRM.dbo.OvertimeYearly (EmpId, [Year], [Minutes], [ModifiedDate])
        VALUES (@EmployeeId, YEAR(@AffectedDate), @NexBalance, GETDATE())
END        

INSERT INTO HRM.dbo.EmployeeCompany(EmpId, CompanyId)
VALUES (@EmployeeId, @ToCompanyId)
