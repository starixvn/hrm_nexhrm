CREATE TABLE [dbo].[EmployeeAttendance] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmpId]           INT             NOT NULL,
    [AttendanceDate]  DATETIME        NOT NULL,
    [ShiftId]         INT             NULL,
    [InFixed]         DATETIME        NULL,
    [OutFixed]        DATETIME        NULL,
    [ExpectedInTime]  DATETIME        NULL,
    [ExpectedOutTime] DATETIME        NULL,
    [LateIn]          INT             NULL,
    [EarlyOut]        INT             NULL,
    [LeaveTypeId]     INT             NULL,
    [Absence]         BIT             NULL,
    [Notes]           NVARCHAR (2000) NULL,
    [TotalWorkHours]  DECIMAL (18, 2) NULL,
    [IsRightShift]    BIT             CONSTRAINT [DF_NexHR_EmployeeAttendance_IsRightShift] DEFAULT ((1)) NULL,
    [IsLocked]        BIT             CONSTRAINT [DF_NexHR_EmployeeAttendance_IsLocked] DEFAULT ((0)) NULL,
    [LeaveDay]        DECIMAL (18, 1) NULL,
    [InsertDate]      DATETIME        NULL,
    [LeaveTypeId2]    INT             NULL,
    CONSTRAINT [PK_NexHR_EmployeeAttendance] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [CIX_EmployeeAttendance_AttendanceDate_EmpID]
    ON [dbo].[EmployeeAttendance]([AttendanceDate] ASC, [EmpId] ASC);

