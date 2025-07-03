DECLARE @EmployeeId INT = 6129
        ,@ToGroupId INT = 3
        ,@NexBalance INT = 0
        ,@AffectedDate DATE = '2025-06-01'
                
DECLARE @UsedLeaveBalance NUMERIC(18, 1)
        ,@LateInEarlyOut INT
        ,@UsedLateInEarlyOut INT
        
        
UPDATE bodb_HRM.dbo.CompanyEmployee
SET 
    IsDeleted = 1
    ,[Enabled] = 0
    ,ResignedDate = @AffectedDate
FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK)
WHERE EmpId = @EmployeeId AND CompanyId = 2

UPDATE bodb_HRM.dbo.CompanyEmployee
SET 
    IsDeleted = 0
    ,[Enabled] = 1
    ,GroupID = @ToGroupId
    ,ResignedDate = NULL
FROM bodb_HRM.dbo.CompanyEmployee WITH (NOLOCK)
WHERE EmpId = @EmployeeId AND CompanyId = 1

DELETE FROM NexHRM.dbo.EmployeeGroup_Employee WHERE EmpID = @EmployeeId

INSERT INTO NexHRM.dbo.EmployeeGroup_Employee (GroupId, EmpID, isMainGroup)
VALUES (@ToGroupId, @EmployeeId, 1)

-- AL
SELECT @UsedLeaveBalance = SUM(ISNULL(DeductDays, 0))
FROM LumiHRM.dbo.LeaveTrans WITH (NOLOCK)
WHERE EmpId = @EmployeeId 
        AND YEAR(FromDate) = YEAR(@AffectedDate) 
        AND ISNULL(Deleted, 0) <> 1

INSERT INTO NexHRM.dbo.LeaveTrans (EmpId, Description, FromDate, ToDate, DeductDays, CreditDays, CreatedDate)
VALUES (@EmployeeId, 'Used in Lumi', @AffectedDate, @AffectedDate, @UsedLeaveBalance, 0, GETDATE())

-- Late In/Early Out
SELECT @LateInEarlyOut = [Minutes]
FROM LumiHRM.dbo.LateInEarlyOutYearly WITH (NOLOCK)
WHERE EmpId = @EmployeeId AND [Year] = YEAR(@AffectedDate)

IF @LateInEarlyOut IS NOT NULL
BEGIN
        INSERT INTO NexHRM.dbo.LateInEarlyOutTrans([EmpId], [WorkDate], [Type], [Minutes], [ModifiedDate], [IsManual])
        SELECT @EmployeeId, [WorkDate], [Type], [Minutes], GETDATE(), [IsManual]
        FROM LumiHRM.dbo.LateInEarlyOutTrans WITH (NOLOCK)
        WHERE EmpId = @EmployeeId
END

UPDATE NexHRM.dbo.LateInEarlyOutYearly
SET [Minutes] = [Minutes] +ISNULL(@LateInEarlyOut, 0)
WHERE EmpId = @EmployeeId AND [Year] = YEAR(@AffectedDate)

-- Overtime
SELECT @UsedLateInEarlyOut = ISNULL(la.[Minutes], 0)
FROM LumiHRM.dbo.Employee AS e WITH (NOLOCK)
LEFT JOIN LumiHRM.dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId AND la.[Year] = YEAR(@AffectedDate)
WHERE e.Id = @EmployeeId

IF @UsedLateInEarlyOut <> 0
BEGIN
        INSERT INTO NexHRM.dbo.OvertimeTrans(EmpId, [Date], [Minutes], [Type], ModifiedDate)
        VALUES (@EmployeeId, @AffectedDate, @UsedLateInEarlyOut, 1, GETDATE())
END
