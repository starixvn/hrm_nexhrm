CREATE TABLE [dbo].[LogAction] (
    [Id]     INT             NOT NULL,
    [Name]   NVARCHAR (4000) NOT NULL,
    [SpName] VARCHAR (500)   NULL,
    CONSTRAINT [PK_LogAction] PRIMARY KEY CLUSTERED ([Id] ASC)
);

