CREATE TABLE [dbo].[HistoryWork] (
	[Id]        INT      IDENTITY (1,1) NOT NULL,
    [EmpId]        INT      NOT NULL,
    [AffectedDate] DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
	[JobtitleId]  INT	NULL,
    [JobtitleName]     VARCHAR(100) NULL,
	[GroupId] INT NULL,
    [GroupName]         VARCHAR(100) NULL,
    [GroupIdOld] INT NULL, 
    CONSTRAINT [PK_HistoryWork] PRIMARY KEY CLUSTERED ([Id] ASC)
);