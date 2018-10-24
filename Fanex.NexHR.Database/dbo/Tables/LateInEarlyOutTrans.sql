CREATE TABLE [dbo].[LateInEarlyOutTrans] (
    [EmpId]            INT      NOT NULL,
    [WorkDate]         DATE     NOT NULL,
    [Type]             TINYINT  NOT NULL,
    [Minutes]          INT      NOT NULL,
    [LeaveDeductionId] INT      NULL,
    [ModifiedBy]       INT      NULL,
    [ModifiedDate]     DATETIME NULL,
    CONSTRAINT [PK_LateInEarlyOutTrans] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC, [Type] ASC)
);

