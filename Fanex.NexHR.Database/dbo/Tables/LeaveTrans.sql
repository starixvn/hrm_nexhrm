CREATE TABLE [dbo].[LeaveTrans] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmpId]             INT             NOT NULL,
    [RequestId]         BIGINT          NULL,
    [Description]       NVARCHAR (4000) NULL,
    [FromDate]          DATETIME        NOT NULL,
    [ToDate]            DATETIME        NULL,
    [DeductDays]        NUMERIC (18, 1) NULL,
    [CreditDays]        NUMERIC (18, 1) NULL,
    [Deleted]           BIT             NULL,
    [CreatedDate]       DATETIME        NULL,
    [CreatedBy]         INT             NULL,
    [ModifiedDate]      DATETIME        NULL,
    [ModifiedBy]        INT             NULL,
    [IsOpenBalance]     BIT             CONSTRAINT [DF_LeaveTrans_IsOpenBalance] DEFAULT ((0)) NULL,
    [IsCloseBalance]    BIT             CONSTRAINT [DF_LeaveTrans_IsCloseBalance] DEFAULT ((0)) NULL,
    [LeaveConversionId] INT             NULL,
    CONSTRAINT [PK_NexHR_LeaveTrans] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_LeaveTrans_EmpID_FromDate]
    ON [dbo].[LeaveTrans]([FromDate] ASC, [EmpId] ASC, [Deleted] ASC)
    INCLUDE([DeductDays], [CreditDays]);

