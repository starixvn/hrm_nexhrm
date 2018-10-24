CREATE TABLE [dbo].[EmployeeGroup_Employee] (
    [Id]            INT IDENTITY (1, 1) NOT NULL,
    [GroupId]       INT NOT NULL,
    [EmpID]         INT NOT NULL,
    [isMainGroup]   BIT CONSTRAINT [DF_EmployeeGroup_Employee_isMainGroup] DEFAULT ((0)) NULL,
    [HandleRequest] BIT NULL,
    CONSTRAINT [PK_EmployeeGroup_Employee] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_EmployeeGroup_Employee_GroupID]
    ON [dbo].[EmployeeGroup_Employee]([GroupId] ASC)
    INCLUDE([EmpID], [HandleRequest]);


GO
CREATE NONCLUSTERED INDEX [IX_EmployeeGroup_Employee_EmpID_GroupID]
    ON [dbo].[EmployeeGroup_Employee]([EmpID] ASC, [GroupId] ASC)
    INCLUDE([isMainGroup], [HandleRequest]);

