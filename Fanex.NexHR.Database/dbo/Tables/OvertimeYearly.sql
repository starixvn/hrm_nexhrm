CREATE TABLE [dbo].[OvertimeYearly] (
    [EmpId]        INT      NOT NULL,
    [Year]         INT      NOT NULL,
    [Minutes]      INT      NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_OvertimeYearly] PRIMARY KEY CLUSTERED ([EmpId] ASC, [Year] ASC)
);

