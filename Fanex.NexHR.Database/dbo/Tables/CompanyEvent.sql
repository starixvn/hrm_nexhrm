CREATE TABLE [dbo].[CompanyEvent] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title]       NVARCHAR (150) NULL,
    [FromDate]    DATE           NULL,
    [ToDate]      DATE           NULL,
    [FromTime]    VARCHAR (8)    NULL,
    [ToTime]      VARCHAR (8)    NULL,
    [Location]    NVARCHAR (100) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [CreatedBy]   INT            NULL,
    [CreatedOn]   DATETIME       NULL,
    CONSTRAINT [PK_CompanyEvent] PRIMARY KEY CLUSTERED ([Id] ASC)
);

