CREATE TABLE [dbo].[EmployeeGroup] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [Code]              VARCHAR (20)  NOT NULL,
    [Name]              NVARCHAR (50) NOT NULL,
    [DepartmentId]      INT           NULL,
    [LeaderId]          INT           NULL,
    [AssistantLeaderId] INT           NULL,
    [Rank]              INT           CONSTRAINT [DF_EmployeeGroup_Rank] DEFAULT ((99)) NULL,
    [CreatedDate]       DATETIME      NULL,
    [CreatedBy]         INT           NULL,
    [ModifiedDate]      DATETIME      NULL,
    [ModifiedBy]        INT           NULL,
    [Deleted]           BIT           NULL,
    [ParentId]          INT           NULL,
    [IsSpecialTeam]     BIT           CONSTRAINT [DF_EmployeeGroup_IsSpecialTeam] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EmployeeGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
);

