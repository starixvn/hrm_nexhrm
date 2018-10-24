CREATE TABLE [dbo].[EmployeeSetting] (
    [EmpId]                        INT NOT NULL,
    [DisplayInfoAndTodoList]       BIT NULL,
    [DisplayTeamAttendence]        BIT NULL,
    [DisplayTeamWorkLoad]          BIT NULL,
    [DisplayRedmineTask]           BIT NULL,
    [DisplayRedmineInputSpendTime] BIT NULL,
    [DisplayRemindLogtime]         BIT NULL,
    CONSTRAINT [PK_EmployeeSetting_1] PRIMARY KEY CLUSTERED ([EmpId] ASC)
);

