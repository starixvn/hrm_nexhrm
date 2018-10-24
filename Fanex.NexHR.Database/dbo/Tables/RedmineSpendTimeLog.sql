CREATE TABLE [dbo].[RedmineSpendTimeLog] (
    [EmpId]       INT            NOT NULL,
    [SpentOn]     DATE           NULL,
    [IssueId]     INT            NULL,
    [Hours]       FLOAT (53)     NULL,
    [Comments]    NVARCHAR (255) NULL,
    [Activity]    INT            NULL,
    [CreatedDate] DATETIME       NULL
);

