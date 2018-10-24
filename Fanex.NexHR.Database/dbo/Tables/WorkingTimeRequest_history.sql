CREATE TABLE [dbo].[WorkingTimeRequest_history] (
    [Id]            BIGINT          NULL,
    [EmpId]         INT             NULL,
    [RequestType]   SMALLINT        NULL,
    [Status]        TINYINT         NULL,
    [IsPlanned]     BIT             NULL,
    [Remark]        NVARCHAR (2000) NULL,
    [ProcessedDate] DATETIME        NULL,
    [ProcessedBy]   INT             NULL,
    [FromDate]      DATETIME        NULL,
    [FromShift]     INT             NULL,
    [ToDate]        DATETIME        NULL,
    [ToShift]       INT             NULL,
    [LeaveTypeID]   INT             NULL,
    [TotalDays]     NUMERIC (5, 1)  NULL,
    [LateInMins]    INT             NULL,
    [EarlyOutMins]  INT             NULL,
    [CreatedDate]   DATETIME        NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedDate]  DATETIME        NULL,
    [ModifiedBy]    INT             NULL,
    [LogDate]       DATETIME        CONSTRAINT [DF_WorkingTimeRequestDetail_Unreject_CreatedDate] DEFAULT (getdate()) NULL,
    [keyid]         BIGINT          IDENTITY (1, 1) NOT NULL,
    [IsCrossDate]   BIT             NULL,
    CONSTRAINT [PK_WorkingTimeRequestDetail_Unreject] PRIMARY KEY CLUSTERED ([keyid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkingTimeRequest_history_RequestId]
    ON [dbo].[WorkingTimeRequest_history]([Id] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1:missing Check in; 2-missing check out; 3- LateIn; 4- EarlyOut; 5- schedule change; 6-leave request', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'WorkingTimeRequest_history', @level2type = N'COLUMN', @level2name = N'RequestType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0: new; 1-pending; 2- approved; 3- rejected', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'WorkingTimeRequest_history', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the late-in minutes ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'WorkingTimeRequest_history', @level2type = N'COLUMN', @level2name = N'LateInMins';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the early out minutes ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'WorkingTimeRequest_history', @level2type = N'COLUMN', @level2name = N'EarlyOutMins';

