CREATE TABLE [dbo].[TOEIC] (
    [EmpId]        INT      NOT NULL,
    [Listening]    INT      NULL,
    [Reading]      INT      NULL,
    [LRExamDate]   DATETIME NULL,
    [Speaking]     INT      NULL,
    [Writing]      INT      NULL,
    [SWExamDate]   DATETIME NULL,
    [OldListening] INT      NULL,
    [OldReading]   INT      NULL,
    [OldSpeaking]  INT      NULL,
    [OldWriting]   INT      NULL,
    [InputDate]    DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    [CreatedBy]    INT      NULL,
    [CreatedDate]  DATETIME NULL,
    [MinimumScore] INT      NULL,
    CONSTRAINT [PK_TOEIC] PRIMARY KEY CLUSTERED ([EmpId] ASC)
);

