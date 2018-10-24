CREATE TABLE [dbo].[EmployeePostnatal] (
    [EmpId]        INT      NOT NULL,
    [FromDate]     DATETIME NOT NULL,
    [ToDate]       DATETIME NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    [Minutes]      INT      CONSTRAINT [DF_EmployeePostnatal_Minutes] DEFAULT ((0)) NOT NULL,
    [Type]         TINYINT  NULL,
    CONSTRAINT [PK_EmployeePostnatal] PRIMARY KEY CLUSTERED ([EmpId] ASC, [FromDate] ASC, [ToDate] ASC)
);

