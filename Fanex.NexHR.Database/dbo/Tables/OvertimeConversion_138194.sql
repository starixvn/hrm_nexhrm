CREATE TABLE [dbo].[OvertimeConversion_138194] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT           NOT NULL,
    [Deduct]       INT           NOT NULL,
    [DeductDate]   DATETIME      NULL,
    [ModifiedBy]   NCHAR (10)    NULL,
    [ModifiedDate] DATETIME      NULL,
    [Description]  VARCHAR (250) NULL,
    CONSTRAINT [PK_OvertimeConversion_138194] PRIMARY KEY CLUSTERED ([Id] ASC)
);





