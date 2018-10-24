CREATE TABLE [dbo].[LateInEarlyOut] (
    [Id]           INT     IDENTITY (1, 1) NOT NULL,
    [WorkDate]     DATE    NULL,
    [Type]         TINYINT NULL,
    [Minutes]      TINYINT NULL,
    [EmpId]        INT     NULL,
    [ModifiedBy]   INT     NULL,
    [ModifiedDate] DATE    NULL,
    CONSTRAINT [PK_LateInEarlyOut] PRIMARY KEY CLUSTERED ([Id] ASC)
);

