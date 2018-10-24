CREATE TABLE [dbo].[TimeRecord] (
    [Id]           NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [EmpID]        INT           NOT NULL,
    [RecordTime]   DATETIME      NOT NULL,
    [InOutType]    TINYINT       NULL,
    [InsertDate]   DATETIME      NULL,
    [ModifiedBy]   INT           NULL,
    [ModifiedDate] DATETIME      NULL,
    [ReaderName]   NVARCHAR (64) NULL,
    [EventName]    NVARCHAR (64) NULL,
    [ReaderId]     INT           NULL,
    CONSTRAINT [PK_NextHR_Record] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_NexHR_TimeRecord_EmpID_RecordTime]
    ON [dbo].[TimeRecord]([RecordTime] ASC, [EmpID] ASC)
    INCLUDE([InOutType]);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'IN record (0) or OUT record (1)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TimeRecord', @level2type = N'COLUMN', @level2name = N'InOutType';

