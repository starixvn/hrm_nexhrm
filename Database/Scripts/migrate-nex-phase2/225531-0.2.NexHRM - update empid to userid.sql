USE NexHRM;
--USE LumiHRM;
--USE NexVisionHRM;
GO

UPDATE c
SET c.CreatedBy = e.UserId
FROM dbo.CompanyEvent c
    INNER JOIN HRM.dbo.Employee e ON c.CreatedBy = e.OldId;
GO

UPDATE att
SET att.EmpId = e.UserId
FROM dbo.EmployeeAttendance att
    INNER JOIN HRM.dbo.Employee e ON att.EmpId = e.OldId;
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmployeeAttendance_bk')
BEGIN
    UPDATE att
    SET att.EmpId = e.UserId
    FROM dbo.EmployeeAttendance_bk att
        INNER JOIN HRM.dbo.Employee e ON att.EmpId = e.OldId;
END
GO

UPDATE empca
SET empca.EmpId = e.UserId
FROM dbo.EmployeeCalendar empca
    INNER JOIN HRM.dbo.Employee e ON empca.EmpId = e.OldId;
GO

UPDATE empca
SET empca.ModifiedBy = e.UserId
FROM dbo.EmployeeCalendar empca
    INNER JOIN HRM.dbo.Employee e ON empca.ModifiedBy = e.OldId;
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmployeeCalendar_bk')
BEGIN
    UPDATE empca
    SET empca.EmpId = e.UserId
    FROM dbo.EmployeeCalendar_bk empca
        INNER JOIN HRM.dbo.Employee e ON empca.EmpId = e.OldId;

    UPDATE empca
    SET empca.ModifiedBy = e.UserId
    FROM dbo.EmployeeCalendar_bk empca
        INNER JOIN HRM.dbo.Employee e ON empca.ModifiedBy = e.OldId;
END
GO

UPDATE ec
SET ec.EmpId = e.UserId
FROM dbo.EmployeeConversation ec
    INNER JOIN HRM.dbo.Employee e ON ec.EmpId = e.OldId;
GO

UPDATE ext
SET ext.Id = e.UserId
FROM dbo.EmployeeExt ext
    INNER JOIN HRM.dbo.Employee e ON ext.Id = e.OldId;
GO

UPDATE eg
SET eg.EmpId = e.UserId
FROM dbo.EmployeeGroup_Employee eg
    INNER JOIN HRM.dbo.Employee e ON eg.EmpId = e.OldId;
GO

UPDATE em
SET em.EmpId = e.UserId
FROM dbo.EmployeeLogTimeMissed em
    INNER JOIN HRM.dbo.Employee e ON em.EmpId = e.OldId;
GO

UPDATE ep
SET ep.EmpId = e.UserId
FROM dbo.EmployeePostnatal ep
    INNER JOIN HRM.dbo.Employee e ON ep.EmpId = e.OldId;
GO

UPDATE ep
SET ep.ModifiedBy = e.UserId
FROM dbo.EmployeePostnatal ep
    INNER JOIN HRM.dbo.Employee e ON ep.ModifiedBy = e.OldId;
GO

UPDATE es
SET es.EmpId = e.UserId
FROM dbo.EmployeeSetting es
    INNER JOIN HRM.dbo.Employee e ON es.EmpId = e.OldId;
GO

UPDATE es
SET es.EmpId = e.UserId
FROM dbo.EmployeeShift es
    INNER JOIN HRM.dbo.Employee e ON es.EmpId = e.OldId;
GO

UPDATE es
SET es.EmpId = e.UserId
FROM dbo.EmployeeSupervisor es
    INNER JOIN HRM.dbo.Employee e ON es.EmpId = e.OldId;
GO

UPDATE es
SET es.ApproverId = e.UserId
FROM dbo.EmployeeSupervisor es
    INNER JOIN HRM.dbo.Employee e ON es.ApproverId = e.OldId;
GO

UPDATE es
SET es.ModifiedBy = e.UserId
FROM dbo.EmployeeSupervisor es
    INNER JOIN HRM.dbo.Employee e ON es.ModifiedBy = e.OldId;
GO

UPDATE wfh
SET wfh.EmpId = e.UserId
FROM dbo.EmployeeWorkFromHome wfh
    INNER JOIN HRM.dbo.Employee e ON wfh.EmpId = e.OldId;
GO

UPDATE wfh
SET wfh.ModifiedBy = e.UserId
FROM dbo.EmployeeWorkFromHome wfh
    INNER JOIN HRM.dbo.Employee e ON wfh.ModifiedBy = e.OldId;
GO

UPDATE enu
SET enu.EmpId = e.UserId
FROM dbo.ExtensionNumbers enu
    INNER JOIN HRM.dbo.Employee e ON enu.EmpId = e.OldId;
GO

UPDATE gs
SET gs.ModifiedBy = e.UserId
FROM dbo.GroupSupervisor gs
    INNER JOIN HRM.dbo.Employee e ON gs.ModifiedBy = e.OldId;
GO

UPDATE gs
SET gs.ApproverId = e.UserId
FROM dbo.GroupSupervisor gs
    INNER JOIN HRM.dbo.Employee e ON gs.ApproverId = e.OldId;
GO

UPDATE hw
SET hw.EmpId = e.UserId
FROM dbo.HistoryWork hw
    INNER JOIN HRM.dbo.Employee e ON hw.EmpId = e.OldId;
GO

UPDATE hw
SET hw.ModifiedBy = e.UserId
FROM dbo.HistoryWork hw
    INNER JOIN HRM.dbo.Employee e ON hw.ModifiedBy = e.OldId;
GO

UPDATE li
SET li.EmpId = e.UserId
FROM dbo.LateInEarlyOut li
    INNER JOIN HRM.dbo.Employee e ON li.EmpId = e.OldId;
GO

UPDATE li
SET li.ModifiedBy = e.UserId
FROM dbo.LateInEarlyOut li
    INNER JOIN HRM.dbo.Employee e ON li.ModifiedBy = e.OldId;
GO

UPDATE li
SET li.EmpId = e.UserId
FROM dbo.LateInEarlyOutTrans li
    INNER JOIN HRM.dbo.Employee e ON li.EmpId = e.OldId;
GO

UPDATE li
SET li.ModifiedBy = e.UserId
FROM dbo.LateInEarlyOutTrans li
    INNER JOIN HRM.dbo.Employee e ON li.ModifiedBy = e.OldId;
GO

UPDATE li
SET li.EmpId = e.UserId
FROM dbo.LateInEarlyOutYearly li
    INNER JOIN HRM.dbo.Employee e ON li.EmpId = e.OldId;
GO

UPDATE li
SET li.ModifiedBy = e.UserId
FROM dbo.LateInEarlyOutYearly li
    INNER JOIN HRM.dbo.Employee e ON li.ModifiedBy = e.OldId;
GO

UPDATE lc
SET lc.EmpId = e.UserId
FROM dbo.LeaveConversion lc
    INNER JOIN HRM.dbo.Employee e ON lc.EmpId = e.OldId;
GO

UPDATE lc
SET lc.ModifiedBy = e.UserId
FROM dbo.LeaveConversion lc
    INNER JOIN HRM.dbo.Employee e ON lc.ModifiedBy = e.OldId;
GO

UPDATE lt
SET lt.EmpId = e.UserId
FROM dbo.LeaveTrans lt
    INNER JOIN HRM.dbo.Employee e ON lt.EmpId = e.OldId;
GO

UPDATE lt
SET lt.CreatedBy = e.UserId
FROM dbo.LeaveTrans lt
    INNER JOIN HRM.dbo.Employee e ON lt.CreatedBy = e.OldId;
GO

UPDATE lt
SET lt.ModifiedBy = e.UserId
FROM dbo.LeaveTrans lt
    INNER JOIN HRM.dbo.Employee e ON lt.ModifiedBy = e.OldId;
GO

UPDATE lt
SET lt.EmpId = e.UserId
FROM dbo.LogReachedWorkingTime lt
    INNER JOIN HRM.dbo.Employee e ON lt.EmpId = e.OldId;
GO

/*
MessageCategoryLog
*/

UPDATE ot
SET ot.CreatedBy = e.UserId
FROM dbo.Overtime ot
    INNER JOIN HRM.dbo.Employee e ON ot.CreatedBy = e.OldId;
GO

UPDATE ot
SET ot.ModifiedBy = e.UserId
FROM dbo.Overtime ot
    INNER JOIN HRM.dbo.Employee e ON ot.ModifiedBy = e.OldId;
GO

UPDATE ot
SET ot.EmpId = e.UserId
FROM dbo.OvertimeConversion ot
    INNER JOIN HRM.dbo.Employee e ON ot.EmpId = e.OldId;
GO

UPDATE ot
SET ot.ModifiedBy = e.UserId
FROM dbo.OvertimeConversion ot
    INNER JOIN HRM.dbo.Employee e ON ot.ModifiedBy = e.OldId;
GO

UPDATE ot
SET ot.EmployeeId = e.UserId
FROM dbo.OvertimeDetail ot
    INNER JOIN HRM.dbo.Employee e ON ot.EmployeeId = e.OldId;
GO

UPDATE ot
SET ot.EmpId = e.UserId
FROM dbo.OvertimeTrans ot
    INNER JOIN HRM.dbo.Employee e ON ot.EmpId = e.OldId;
GO

UPDATE ot
SET ot.ModifiedBy = e.UserId
FROM dbo.OvertimeTrans ot
    INNER JOIN HRM.dbo.Employee e ON ot.ModifiedBy = e.OldId;
GO

UPDATE ot
SET ot.EmpId = e.UserId
FROM dbo.OvertimeYearly ot
    INNER JOIN HRM.dbo.Employee e ON ot.EmpId = e.OldId;
GO

UPDATE ot
SET ot.ModifiedBy = e.UserId
FROM dbo.OvertimeYearly ot
    INNER JOIN HRM.dbo.Employee e ON ot.ModifiedBy = e.OldId;
GO

UPDATE rst
SET rst.EmpId = e.UserId
FROM dbo.RedmineSpendTimeLog rst
    INNER JOIN HRM.dbo.Employee e ON rst.EmpId = e.OldId;
GO

UPDATE tr
SET tr.EmpId = e.UserId
FROM dbo.TimeRecord tr
    INNER JOIN HRM.dbo.Employee e ON tr.EmpId = e.OldId;
GO

UPDATE tr
SET tr.EmpId = e.UserId
FROM dbo.TimeRecordOld tr
    INNER JOIN HRM.dbo.Employee e ON tr.EmpId = e.OldId;
GO

UPDATE tr
SET tr.EmpId = e.UserId
FROM dbo.tmpEmployeeAttendance tr
    INNER JOIN HRM.dbo.Employee e ON tr.EmpId = e.OldId;
GO

UPDATE wfh
SET wfh.EmpId = e.UserId
FROM dbo.WorkFromHomeRequest wfh
    INNER JOIN HRM.dbo.Employee e ON wfh.EmpId = e.OldId;
GO

UPDATE wfh
SET wfh.ProcessedBy = e.UserId
FROM dbo.WorkFromHomeRequest wfh
    INNER JOIN HRM.dbo.Employee e ON wfh.ProcessedBy = e.OldId;
GO

UPDATE wfh
SET wfh.CreatedBy = e.UserId
FROM dbo.WorkFromHomeRequest wfh
    INNER JOIN HRM.dbo.Employee e ON wfh.CreatedBy = e.OldId;
GO

UPDATE wfh
SET wfh.ModifiedBy = e.UserId
FROM dbo.WorkFromHomeRequest wfh
    INNER JOIN HRM.dbo.Employee e ON wfh.ModifiedBy = e.OldId;
GO

/*
WorkingTimeRequest
*/

UPDATE r
SET r.EmpId = e.UserId
FROM dbo.WorkingTimeRequest r
    INNER JOIN HRM.dbo.Employee e ON r.EmpId = e.OldId;
GO

UPDATE r
SET r.ProcessedBy = e.UserId
FROM dbo.WorkingTimeRequest r
    INNER JOIN HRM.dbo.Employee e ON r.ProcessedBy = e.OldId;
GO

UPDATE r
SET r.CreatedBy = e.UserId
FROM dbo.WorkingTimeRequest r
    INNER JOIN HRM.dbo.Employee e ON r.CreatedBy = e.OldId;
GO

UPDATE r
SET r.ModifiedBy = e.UserId
FROM dbo.WorkingTimeRequest r
    INNER JOIN HRM.dbo.Employee e ON r.ModifiedBy = e.OldId;
GO

UPDATE r
SET r.EmpId = e.UserId
FROM dbo.WorkingTimeRequest_history r
    INNER JOIN HRM.dbo.Employee e ON r.EmpId = e.OldId;
GO

UPDATE r
SET r.ProcessedBy = e.UserId
FROM dbo.WorkingTimeRequest_history r
    INNER JOIN HRM.dbo.Employee e ON r.ProcessedBy = e.OldId;
GO

UPDATE r
SET r.CreatedBy = e.UserId
FROM dbo.WorkingTimeRequest_history r
    INNER JOIN HRM.dbo.Employee e ON r.CreatedBy = e.OldId;
GO

UPDATE r
SET r.ModifiedBy = e.UserId
FROM dbo.WorkingTimeRequest_history r
    INNER JOIN HRM.dbo.Employee e ON r.ModifiedBy = e.OldId;
GO

/*
MessageCategoryLog
*/
UPDATE mcl
SET mcl.JsonObject = REPLACE(mcl.JsonObject, 
    CONCAT('"EmpId":', CAST(e.OldId AS VARCHAR(10))), 
    CONCAT('"EmpId":', CAST(e.UserId AS VARCHAR(10)))
)
FROM dbo.MessageCategoryLog mcl
INNER JOIN HRM.dbo.Employee e ON mcl.JsonObject LIKE CONCAT('%"EmpId":', CAST(e.OldId AS VARCHAR(10)), '%');
GO
