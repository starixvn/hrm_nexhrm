CREATE TABLE [dbo].[Shift] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Code]              VARCHAR (10)   NOT NULL,
    [Name]              NVARCHAR (100) NOT NULL,
    [BeginCheckInTime]  NCHAR (5)      NOT NULL,
    [EndCheckInTime]    NCHAR (5)      NOT NULL,
    [BeginCheckOutTime] NCHAR (5)      NOT NULL,
    [EndCheckOutTime]   NCHAR (5)      NOT NULL,
    [BreakTimeMins]     INT            NOT NULL,
    [OrderNo]           TINYINT        NOT NULL,
    [BeginBreakTime]    NCHAR (5)      NULL,
    [EndBreakTime]      NCHAR (5)      NULL,
    [IsInAfterDay]      BIT            CONSTRAINT [DF_Shift_IsInAfterDay] DEFAULT ((0)) NOT NULL,
    [IsOutAfterDay]     BIT            CONSTRAINT [DF_Shift_IsOutAfterDay] DEFAULT ((0)) NOT NULL,
    [ModifiedBy]        INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    [IsDisabled]        BIT            CONSTRAINT [DF_Shift_IsDisabled] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Shift] PRIMARY KEY CLUSTERED ([Id] ASC)
);

