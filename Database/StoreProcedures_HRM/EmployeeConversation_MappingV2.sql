CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_MappingV2]
    @ConversationId VARCHAR(255),
    @UserName VARCHAR(500),
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
    Created: 20250613@Henry.Dinh
    Task: update conversation id for employee
    DB: HRM, NexHRM, LumiHRM, NexVisionHRM
    Original: 
    Revisions:    
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    ------------------------------------------------------------------
    -- 1. LumiHRM
    ------------------------------------------------------------------
    DECLARE @CleanUser_Lumi VARCHAR(500) = REPLACE(@UserName, '@sabawork.com', '')
    DECLARE @EmpId_Lumi INT

    SELECT TOP 1 @EmpId_Lumi = Id
    FROM LumiHRM.dbo.Employee WITH (NOLOCK)
    WHERE isDeleted = 0
      AND REPLACE(ISNULL(DomainUsername, ''), 'lumiplus\', '') = @CleanUser_Lumi

    IF @EmpId_Lumi IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM LumiHRM.dbo.EmployeeConversation WITH (NOLOCK) WHERE EmpId = @EmpId_Lumi)
        BEGIN
            UPDATE LumiHRM.dbo.EmployeeConversation WITH (ROWLOCK, UPDLOCK)
            SET ConversationId = @ConversationId,
                ModifiedDate = dbo.fnc_GetClientDateTime()
            WHERE EmpId = @EmpId_Lumi AND ConversationId <> @ConversationId
        END
        ELSE
        BEGIN
            INSERT INTO LumiHRM.dbo.EmployeeConversation (EmpId, ConversationId, ModifiedDate)
            VALUES (@EmpId_Lumi, @ConversationId, dbo.fnc_GetClientDateTime())
        END
    END

    ------------------------------------------------------------------
    -- 2. NexHRM
    ------------------------------------------------------------------
    DECLARE @CleanUser_Nex VARCHAR(500) = REPLACE(@UserName, '@starixsoft.com', '')
    DECLARE @EmpId_Nex INT

    SELECT TOP 1 @EmpId_Nex = Id
    FROM NexHRM.dbo.Employee WITH (NOLOCK)
    WHERE isDeleted = 0
      AND REPLACE(ISNULL(DomainUsername, ''), 'spvn\', '') = @CleanUser_Nex

    IF @EmpId_Nex IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM NexHRM.dbo.EmployeeConversation WITH (NOLOCK) WHERE EmpId = @EmpId_Nex)
        BEGIN
            UPDATE NexHRM.dbo.EmployeeConversation WITH (ROWLOCK, UPDLOCK)
            SET ConversationId = @ConversationId,
                ModifiedDate = dbo.fnc_GetClientDateTime()
            WHERE EmpId = @EmpId_Nex AND ConversationId <> @ConversationId
        END
        ELSE
        BEGIN
            INSERT INTO NexHRM.dbo.EmployeeConversation (EmpId, ConversationId, ModifiedDate)
            VALUES (@EmpId_Nex, @ConversationId, dbo.fnc_GetClientDateTime())
        END
    END

    ------------------------------------------------------------------
    -- 3. NexVisionHRM
    ------------------------------------------------------------------
    DECLARE @CleanUser_Vision VARCHAR(500) = REPLACE(@UserName, '@starixsoft.com', '')
    DECLARE @EmpId_Vision INT

    SELECT TOP 1 @EmpId_Vision = Id
    FROM NexVisionHRM.dbo.Employee WITH (NOLOCK)
    WHERE isDeleted = 0
      AND ISNULL(DomainUsername, '') = @CleanUser_Vision -- không có prefix

    IF @EmpId_Vision IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM NexVisionHRM.dbo.EmployeeConversation WITH (NOLOCK) WHERE EmpId = @EmpId_Vision)
        BEGIN
            UPDATE NexVisionHRM.dbo.EmployeeConversation WITH (ROWLOCK, UPDLOCK)
            SET ConversationId = @ConversationId,
                ModifiedDate = dbo.fnc_GetClientDateTime()
            WHERE EmpId = @EmpId_Vision AND ConversationId <> @ConversationId
        END
        ELSE
        BEGIN
            INSERT INTO NexVisionHRM.dbo.EmployeeConversation (EmpId, ConversationId, ModifiedDate)
            VALUES (@EmpId_Vision, @ConversationId, dbo.fnc_GetClientDateTime())
        END
    END
END
