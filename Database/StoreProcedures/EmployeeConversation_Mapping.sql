CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_Mapping] 
AS
/*
Created: 20140604@Quinn.Vo
Task: Insert Leave deduction.
DB: NexHRM
Original: 

Revisions: 
	- 20250317@Henry.Dinh: Get latest conversationid by ModifiedDate
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @PrefixDomain VARCHAR(50) = 'spvn\'
		,@EmailDomain VARCHAR(50) = '@starixsoft.com'
		,@BotName VARCHAR(50) = 'nexhrbot'

	DECLARE @ModifiedDate DATETIME
	CREATE TABLE #tblConversationUser(
		ConversationId VARCHAR(255)
		,Email VARCHAR(500)
	)

	SELECT @ModifiedDate = MAX(ISNULL(ModifiedDate, '2020-01-01'))
	FROM dbo.EmployeeConversation WITH (NOLOCK)

	INSERT INTO #tblConversationUser(ConversationId, Email)
	SELECT 
        ConversationId,
        UserName
    FROM (
        SELECT 
            co.ConversationId,
            cou.UserName,
            ROW_NUMBER() OVER (
				PARTITION BY REPLACE(cou.UserName, @EmailDomain, '')
				ORDER BY co.ModifiedDate DESC
   			) as RN
        FROM NexBot.dbo.Conversation AS co WITH (NOLOCK)
        INNER JOIN NexBot.dbo.ConversationUser AS cou WITH (NOLOCK) 
            ON co.ConversationId = cou.ConversationId
        INNER JOIN NexBot.dbo.ConversationBot AS bot WITH (NOLOCK) 
            ON co.ConversationBotId = bot.ConversationBotId
        WHERE bot.Name = @BotName
    ) t
    WHERE t.RN = 1

	UPDATE ec
	SET ec.ConversationId = cou.ConversationId
		,ec.ModifiedDate = dbo.fnc_GetClientDateTime()
	FROM dbo.EmployeeConversation AS ec
	INNER JOIN dbo.Employee AS e ON ec.EmpId = e.Id
	INNER JOIN #tblConversationUser AS cou ON 
	REPLACE(ISNULL(e.DomainUsername, ''), @PrefixDomain, '') = REPLACE(cou.Email, @EmailDomain, '')
	WHERE ec.ConversationId <> cou.ConversationId

	INSERT INTO dbo.EmployeeConversation(EmpId, ConversationId, ModifiedDate)
	SELECT e.Id, cou.ConversationId, dbo.fnc_GetClientDateTime()
	FROM #tblConversationUser AS cou
	INNER JOIN dbo.Employee AS e WITH (NOLOCK)
		ON e.isDeleted = 0
		AND REPLACE(ISNULL(e.DomainUsername, ''), @PrefixDomain, '') = REPLACE(cou.Email, @EmailDomain, '')
	LEFT JOIN dbo.EmployeeConversation AS ec ON ec.EmpId = e.Id
	WHERE ec.EmpId IS NULL

	DROP TABLE #tblConversationUser
END
GO
