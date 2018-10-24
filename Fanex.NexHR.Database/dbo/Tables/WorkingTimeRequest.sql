CREATE TABLE [dbo].[WorkingTimeRequest] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmpId]         INT             NOT NULL,
    [RequestType]   SMALLINT        NOT NULL,
    [Status]        TINYINT         NOT NULL,
    [IsPlanned]     BIT             NULL,
    [Remark]        NVARCHAR (2000) NULL,
    [ProcessedDate] DATETIME        NULL,
    [ProcessedBy]   INT             NULL,
    [FromDate]      DATETIME        NOT NULL,
    [FromShift]     INT             NULL,
    [ToDate]        DATETIME        NULL,
    [ToShift]       INT             NULL,
    [LeaveTypeID]   INT             NULL,
    [TotalDays]     NUMERIC (5, 1)  NULL,
    [LateInMins]    INT             NULL,
    [EarlyOutMins]  INT             NULL,
    [CreatedDate]   DATETIME        NOT NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedDate]  DATETIME        NULL,
    [ModifiedBy]    INT             NULL,
    [IsCrossDate]   BIT             NULL,
    CONSTRAINT [PK_WorkingTimeRequest] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkingTimeRequest_100]
    ON [dbo].[WorkingTimeRequest]([CreatedDate] ASC, [EmpId] ASC, [RequestType] ASC, [Status] ASC)
    INCLUDE([ProcessedDate], [ProcessedBy]);


GO
CREATE NONCLUSTERED INDEX [IX_WorkingTimeRequest_101]
    ON [dbo].[WorkingTimeRequest]([FromDate] ASC, [EmpId] ASC, [RequestType] ASC, [Status] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1:missing Check in; 2-missing check out; 3- LateIn; 4- EarlyOut; 5- schedule change; 6-leave request', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'WorkingTimeRequest', @level2type = N'COLUMN', @level2name = N'RequestType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0: new; 1-pending; 2- approved; 3- rejected', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'WorkingTimeRequest', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the late-in minutes ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'WorkingTimeRequest', @level2type = N'COLUMN', @level2name = N'LateInMins';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'the early out minutes ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'WorkingTimeRequest', @level2type = N'COLUMN', @level2name = N'EarlyOutMins';

