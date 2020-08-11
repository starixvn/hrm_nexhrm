CREATE TABLE [dbo].[Overtime_138194] (
    [OvertimeId]   INT             IDENTITY (1, 1) NOT NULL,
    [Description]  NVARCHAR (4000) NULL,
    [Status]       INT             NULL,
    [CreatedBy]    INT             NULL,
    [CreatedDate]  DATETIME        NULL,
    [ModifiedBy]   INT             NULL,
    [ModifiedDate] DATETIME        NULL,
    PRIMARY KEY CLUSTERED ([OvertimeId] ASC)
);

