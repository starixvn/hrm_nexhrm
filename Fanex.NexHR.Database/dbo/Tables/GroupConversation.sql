CREATE TABLE [dbo].[GroupConversation] (
    [ConversationId] VARCHAR (255)  NOT NULL,
    [GroupName]      NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_GroupConversation] PRIMARY KEY CLUSTERED ([ConversationId] ASC)
);

