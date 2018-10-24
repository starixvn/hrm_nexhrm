CREATE TABLE [dbo].[LeaveType] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Code]              VARCHAR (10)   NOT NULL,
    [Name]              NVARCHAR (100) NOT NULL,
    [IsDefault]         BIT            CONSTRAINT [DF_Table_1_isDefault] DEFAULT ((0)) NOT NULL,
    [IsSalaryDeduction] BIT            NULL,
    [IsALDeduction]     BIT            NULL,
    [IsHoliday]         BIT            NULL,
    [isShown]           BIT            CONSTRAINT [DF_LeaveType_isShown] DEFAULT ((1)) NULL,
    [IsInsuranceLeave]  BIT            NULL,
    CONSTRAINT [PK_LeaveType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

