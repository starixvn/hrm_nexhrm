CREATE TABLE [dbo].[Department] (
    [DepartmentId]    INT            IDENTITY (1, 1) NOT NULL,
    [Code]            VARCHAR (20)   NOT NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [ParentId]        INT            NULL,
    [ManagerId]       INT            NULL,
    [DepartmentLevel] INT            NULL,
    [DisplayOrder]    INT            NULL,
    [IsDisabled]      BIT            NULL,
    [CreatedBy]       INT            NULL,
    [CreatedDate]     DATETIME       NULL,
    [ModifiedBy]      INT            NULL,
    [ModifiedDate]    DATETIME       NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentId] ASC)
);

