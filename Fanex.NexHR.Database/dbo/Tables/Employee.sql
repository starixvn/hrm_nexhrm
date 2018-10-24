CREATE TABLE [dbo].[Employee] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [Code]               VARCHAR (20)    NOT NULL,
    [DomainUsername]     VARCHAR (50)    NULL,
    [FullName]           NVARCHAR (50)   NOT NULL,
    [NickName]           NVARCHAR (20)   NULL,
    [DOB]                SMALLDATETIME   NOT NULL,
    [Email]              VARCHAR (50)    NULL,
    [JobTitleId]         INT             NULL,
    [Enabled]            BIT             CONSTRAINT [DF_Table_1_Enable] DEFAULT ((1)) NOT NULL,
    [Phone]              VARCHAR (50)    NULL,
    [Gender]             TINYINT         NULL,
    [GroupId]            INT             NULL,
    [Roles]              VARCHAR (20)    CONSTRAINT [DF_Employee_Role] DEFAULT ((0)) NULL,
    [ManagerId]          INT             NULL,
    [JoinedDate]         SMALLDATETIME   NULL,
    [ConfirmedDate]      SMALLDATETIME   NULL,
    [ResignedDate]       SMALLDATETIME   NULL,
    [Skype]              VARCHAR (100)   NULL,
    [Yahoo]              VARCHAR (100)   NULL,
    [Mariage]            BIT             NULL,
    [LastLogin]          DATETIME        NULL,
    [AnnualLeaveBalance] NUMERIC (18, 1) NULL,
    [CreatedDate]        DATETIME        NOT NULL,
    [CreatedBy]          INT             NULL,
    [ModifiedDate]       DATETIME        NULL,
    [ModifiedBy]         INT             NULL,
    [isDeleted]          BIT             NULL,
    [PermanentAddress]   NVARCHAR (250)  NULL,
    [CurrentAddress]     NVARCHAR (250)  NULL,
    [IdCardNumber]       VARCHAR (20)    NULL,
    [PassportNo]         VARCHAR (35)    NULL,
    [PassportExp]        DATETIME        NULL,
    [GeoMapLatitude]     VARCHAR (50)    NULL,
    [GeoMapLongtitude]   VARCHAR (50)    NULL,
    [DistanceToCompany]  VARCHAR (10)    NULL,
    [EmployeeCard]       VARCHAR (10)    NULL,
    [IssuedDate]         DATETIME        NULL,
    [IssuedPlace]        NVARCHAR (250)  NULL,
    [IsSpecialUser]      BIT             CONSTRAINT [DF_Employee_IsSpecialUser] DEFAULT ((0)) NOT NULL,
    [CompanyDate]        DATE            NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Employee_EmpCode]
    ON [dbo].[Employee]([Code] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N' 0-member,1-ass, 2-leader, 3-hr, 4-management, 5-admin', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Employee', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0:female, 1: male, 2: unknown', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Employee', @level2type = N'COLUMN', @level2name = N'Gender';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0-member,1-assitant leader, 2-leader, 3-hr, 4-admin', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Employee', @level2type = N'COLUMN', @level2name = N'Roles';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The current number of annual days', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Employee', @level2type = N'COLUMN', @level2name = N'AnnualLeaveBalance';

