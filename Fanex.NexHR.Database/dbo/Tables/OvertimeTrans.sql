CREATE TABLE [dbo].[OvertimeTrans] (
    [EmpId]             INT      NOT NULL,
    [Date]              DATE     NOT NULL,
    [Type]              TINYINT  NOT NULL,
    [Minutes]           INT      NOT NULL,
    [OvertimeId]        INT      NULL,
    [LeaveConversionId] INT      NULL,
    [ModifiedBy]        INT      NULL,
    [ModifiedDate]      DATETIME NULL
);

