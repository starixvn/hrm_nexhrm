CREATE TABLE [dbo].[Log] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [LogActionID]   INT             NOT NULL,
    [LogDate]       DATETIME        NOT NULL,
    [Description]   NVARCHAR (4000) NOT NULL,
    [TrackingValue] NVARCHAR (MAX)  NULL,
    [UserID]        INT             NOT NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC)
);

