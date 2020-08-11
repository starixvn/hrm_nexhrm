CREATE TABLE [dbo].[OvertimeDetail_138194] (
    [OvertimeId] INT             NOT NULL,
    [EmployeeId] INT             NOT NULL,
    [Date]       DATE            NOT NULL,
    [Hour]       NUMERIC (18, 1) NOT NULL,
    [NexHour]    NUMERIC (18, 1) NOT NULL,
    [Note]       NVARCHAR (250)  NULL,
    [Verified]   BIGINT          NULL,
    CONSTRAINT [PK_OvertimeDetail_138194] PRIMARY KEY CLUSTERED ([OvertimeId] ASC, [EmployeeId] ASC, [Date] ASC)
);

