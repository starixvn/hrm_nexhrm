CREATE TABLE [dbo].[OvertimeDetail] (
    [OvertimeId] INT             NOT NULL,
    [EmployeeId] INT             NOT NULL,
    [Hour]       NUMERIC (18, 1) NOT NULL,
    CONSTRAINT [PK_OvertimeDetail] PRIMARY KEY CLUSTERED ([OvertimeId] ASC, [EmployeeId] ASC)
);

