ALTER TABLE [bodb_HRM].[dbo].[CompanyEmployee]
ADD
    [DomainUsername] VARCHAR(50) NULL,
    [Email] VARCHAR(50) NULL,
    [GroupId] INT NULL,
    [Roles] VARCHAR(20) NULL,
    [CompanyDate] SMALLDATETIME NULL,
    [ConfirmedDate] SMALLDATETIME NULL,
    [ResignedDate] SMALLDATETIME NULL,
    [NickName2] NVARCHAR(20) NULL,
    [IsSpecialUser] BIT NULL,
    [IsDeleted] BIT NULL,
    [Enabled] BIT NULL,
    [SalaryGroupId] SMALLINT NULL,
    [Password] NVARCHAR(100) NULL;
GO

ALTER TABLE [bodb_HRM].[dbo].[Employee]
ADD 
    [Telegram]            VARCHAR(100) NULL,
    [GeoMapLatitude]      VARCHAR(50) NULL,
    [GeoMapLongtitude]    VARCHAR(50) NULL,
    [DistanceToCompany]   VARCHAR(10) NULL,
    [ContactName]         NVARCHAR(50) NULL,
    [UserRelationship]    NVARCHAR(20) NULL,
    [ContactPhone]        VARCHAR(50) NULL,
    [ContactAddress]      NVARCHAR(250) NULL,
    [EducationBackground] NVARCHAR(500) NULL,
    [SocialInsuranceNo]   VARCHAR(50) NULL,
    [RefId]               INT NULL,
    [RefCompanyId]        INT NULL;
GO


