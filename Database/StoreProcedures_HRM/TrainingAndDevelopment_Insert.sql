USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[TrainingAndDevelopment_Insert]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_Insert] 
    @EmpId INT
    ,@CreatedBy INT
    ,@Publish NVARCHAR(20)
    ,@ConductedBy NVARCHAR(100) = NULL
    ,@TypeId INT
    ,@Name NVARCHAR(200)
    ,@Organization NVARCHAR(200)
    ,@Description NVARCHAR(500) = NULL
    ,@StartDate DATETIME = NULL
    ,@EndDate DATETIME = NULL
    ,@ExamDate DATETIME = NULL
    ,@ExpirationDate DATETIME = NULL
	,@Certificate VARCHAR(MAX) = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230417@Brian.Nguyen
Task: Insert Training And Development
DB: NexHRM
Original: 

Revisions:  
- 20241202@Lilac: Add certificate upload
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[TrainingAndDevelopment](
        [EmpId]
           ,[ConductedBy]
           ,[TypeId]
           ,[Name]
           ,[Organization]
           ,[StartDate]
           ,[EndDate]
           ,[ExamDate]
           ,[ExpirationDate]
           ,[CreatedDate]
		   ,[CreatedBy]
		   ,[Description]
		   ,[Publish]
		   ,[Certificate]
        )
    VALUES (
        @EmpId
        ,@ConductedBy
        ,@TypeId
        ,@Name
        ,@Organization
        ,@StartDate
        ,@EndDate
        ,@ExamDate
        ,@ExpirationDate
		, dbo.fnc_GetClientDateTime()
		,@CreatedBy
		,@Description
		,@Publish
		,@Certificate
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END

GO
