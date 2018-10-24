CREATE TABLE [dbo].[EmployeeCalendar] (
    [Id]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmpId]                 INT             NOT NULL,
    [WorkDate]              DATETIME        NOT NULL,
    [ShiftId]               INT             NULL,
    [IsHoliday]             BIT             CONSTRAINT [DF_NexHR_EmployeeCalendar_IsHoliday] DEFAULT ((0)) NULL,
    [IsOffDay]              BIT             CONSTRAINT [DF_NexHR_EmployeeCalendar_isOffDay] DEFAULT ((0)) NULL,
    [IsLeaveDay]            BIT             CONSTRAINT [DF_NexHR_EmployeeCalendar_IsLeaveDay] DEFAULT ((0)) NULL,
    [LeaveTypeId]           INT             NULL,
    [LateIn]                INT             NULL,
    [EarlyOut]              INT             NULL,
    [RequestedCheckinTime]  DATETIME        NULL,
    [RequestedCheckoutTime] DATETIME        NULL,
    [ChangeShiftId]         BIGINT          NULL,
    [LeaveDay]              DECIMAL (18, 1) NULL,
    [ModifiedDate]          DATETIME        NULL,
    [ModifiedBy]            INT             NULL,
    [LeaveTypeId2]          INT             NULL,
    CONSTRAINT [PK_NexHR_EmployeeCalendar] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [CIX_EmployeeCalendar_WorkDate_EmpID]
    ON [dbo].[EmployeeCalendar]([WorkDate] ASC, [EmpId] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Mã số ca (TS_Shift)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'EmployeeCalendar', @level2type = N'COLUMN', @level2name = N'ShiftId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ngày được nghỉ theo phân ca', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'EmployeeCalendar', @level2type = N'COLUMN', @level2name = N'IsHoliday';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Ngày xin nghỉ trước của NV', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'EmployeeCalendar', @level2type = N'COLUMN', @level2name = N'IsLeaveDay';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Loại nghỉ xin trước', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'EmployeeCalendar', @level2type = N'COLUMN', @level2name = N'LeaveTypeId';

