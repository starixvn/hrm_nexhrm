CREATE TABLE [dbo].[LeaveConversion] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT             NOT NULL,
    [Date]         DATETIME        NOT NULL,
    [IsAnnual]     BIT             NOT NULL,
    [LeaveDays]    NUMERIC (18, 1) NOT NULL,
    [Description]  NVARCHAR (2000) NULL,
    [IsDeleted]    BIT             CONSTRAINT [DF_LeaveDeduction_IsDeleted] DEFAULT ((0)) NOT NULL,
    [ModifiedBy]   INT             NULL,
    [ModifiedDate] DATETIME        NULL,
    [Type]         TINYINT         CONSTRAINT [DF_LeaveConversion_Type] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_LeaveDeduction] PRIMARY KEY CLUSTERED ([Id] ASC)
);

