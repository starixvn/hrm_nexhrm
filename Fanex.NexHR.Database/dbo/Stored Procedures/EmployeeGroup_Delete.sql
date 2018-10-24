
/****** Object:  Stored Procedure dbo.EmployeeGroup_Delete    Script Date: 3/13/2016 3:21:40 PM ******/

CREATE PROCEDURE [dbo].[EmployeeGroup_Delete] @Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: soft delete a group 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                    INNER JOIN Employee e WITH (NOLOCK) ON eg.EmpID = e.ID
                WHERE eg.GroupID = @Id
                    AND ISNULL(isDeleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 100
            ,@ErrMsg = 'The group is not empty so cannot delete it'

        RETURN
    END

    IF OBJECT_ID('tempdb..#RoleID') IS NOT NULL
    BEGIN
        DROP TABLE #RoleID
    END

    CREATE TABLE #RoleID (RoleID INT)

    DECLARE @LeaderID INT = NULL
        ,@AssistantLeaderID INT = NULL
        ,@Roles VARCHAR(20) = NULL
        ,@tmpRoles VARCHAR(20)

    SELECT @LeaderID = g.LeaderId
        ,@AssistantLeaderID = g.AssistantLeaderId
    FROM [dbo].[EmployeeGroup] g WITH (NOLOCK)
    WHERE g.Id = @Id

    IF @LeaderID IS NOT NULL
        AND @LeaderID <> 0
    BEGIN
        SET @Roles = (
                SELECT TOP 1 e.Roles
                FROM [dbo].[Employee] e WITH (NOLOCK)
                WHERE e.Id = @LeaderId
                )

        IF @Roles IS NOT NULL
        BEGIN
            DELETE
            FROM #RoleID

            INSERT INTO #RoleID (RoleID)
            SELECT val
            FROM fnc_strSplit(@Roles, ',')

            IF EXISTS (
                    SELECT TOP 1 r.RoleID
                    FROM #RoleID r
                    WHERE r.RoleID = 2
                    )
            BEGIN
                SET @tmpRoles = NULL

                DELETE
                FROM #RoleID
                WHERE RoleID = 2

                SELECT @tmpRoles = COALESCE(@tmpRoles + ',', '') + CONVERT(VARCHAR, r.RoleID)
                FROM #RoleID r WITH (NOLOCK)
                ORDER BY r.RoleID

                UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
                SET Roles = @tmpRoles
                WHERE Id = @LeaderId
            END
        END
    END

    IF @AssistantLeaderID IS NOT NULL
        AND @AssistantLeaderID <> 0
    BEGIN
        SET @Roles = (
                SELECT TOP 1 e.Roles
                FROM [dbo].[Employee] e WITH (NOLOCK)
                WHERE e.Id = @AssistantLeaderID
                )

        IF @Roles IS NOT NULL
        BEGIN
            DELETE
            FROM #RoleID

            INSERT INTO #RoleID (RoleID)
            SELECT val
            FROM fnc_strSplit(@Roles, ',')

            IF EXISTS (
                    SELECT TOP 1 r.RoleID
                    FROM #RoleID r
                    WHERE r.RoleID = 1
                    )
            BEGIN
                SET @tmpRoles = NULL

                DELETE
                FROM #RoleID
                WHERE RoleID = 1

                SELECT @tmpRoles = COALESCE(@tmpRoles + ',', '') + CONVERT(VARCHAR, r.RoleID)
                FROM #RoleID r
                ORDER BY r.RoleID

                UPDATE [dbo].[Employee]
                SET Roles = @tmpRoles
                WHERE Id = @AssistantLeaderID
            END
        END
    END

    UPDATE [dbo].[EmployeeGroup] WITH (ROWLOCK, UPDLOCK)
    SET Deleted = 1
        ,ModifiedDate = GETDATE()
        ,ModifiedBy = @ModifiedBy
    WHERE Id = @Id

    SET @ErrCode = 0

    IF OBJECT_ID('tempdb..#RoleID') IS NOT NULL
    BEGIN
        DROP TABLE #RoleID
    END
END

