USE [HRM]
GO

CREATE OR ALTER FUNCTION [dbo].[fnc_GetClientDateTime]()
RETURNS DATETIME
AS
BEGIN
    DECLARE @ClientTimeZone NVARCHAR(50) = 'SE Asia Standard Time'; /* SE Asia Standard Time (UTC+7) */
    DECLARE @CurrentTimeZone NVARCHAR(50) = 'SA Western Standard Time'; /* SA Western Standard Time (UTC-4) */
    
    RETURN GETDATE() AT TIME ZONE @CurrentTimeZone AT TIME ZONE @ClientTimeZone
END
GO

CREATE OR ALTER FUNCTION [dbo].[fnc_strSplit] ( @string nvarchar( 4000), @splitter char( 1) )
returns @res table ( id int primary key, rank int, val nvarchar( 4000) )
as
begin
     if substring ( @string, len ( @string), 1)<>@splitter
          set @string= @string+@splitter
     declare @start int, @word nvarchar(4000), @charindex int, @i int
     set @i=1
     set @start=1
     set @charindex= charindex( @splitter, @string, @start)
     while (@charindex <> 0)begin
          set @word= substring( @string, @start, @charindex - @start)
          set @start= @charindex +1
          set @charindex= charindex( @splitter, @string, @start)
          insert into @res  values ( @start, @i, LTrim(Rtrim(@word)))
          set @i=@i+1
     end
     return
end
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_Delete] @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20231120@Lilac.Nguyen
Task: delete a applicant 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

	
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Applicant WITH (NOLOCK)
        WHERE ApplicantId = @Id
        )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The applicant does not exists '

        RETURN
    END

	DELETE FROM dbo.Applicant 
	WHERE ApplicantId = @Id;

	 SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_Get] @FromDate DATETIME  = NULL
    ,@ToDate DATETIME = NULL
    ,@FullName NVARCHAR(100) = ''
    ,@Status INT = 0
	,@PositionId INT = 0
	,@PageNumber INT
    ,@PageSize INT
    ,@TotalRow INT OUTPUT
AS
/*
Created: 20231115@Lilac.Nguyen
Task: Get all applicant.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT @TotalRow = COUNT(*)
    FROM dbo.Applicant AS ap WITH (NOLOCK)
        INNER JOIN dbo.Position AS p WITH (NOLOCK) ON ap.PositionId = p.PositionId
    WHERE ap.FullName LIKE '%' + @FullName + '%' AND
        DATEDIFF(DAY, ISNULL(@FromDate, '01-01-1900'), ap.CreatedDate) >= 0 AND
        DATEDIFF(DAY, ISNULL(@ToDate, dbo.fnc_GetClientDateTime()), ap.CreatedDate) <= 0 AND
        (@Status = 0 OR ap.Status = @Status) AND
        (@PositionId = 0 OR ap.PositionId = @PositionId);

    SELECT *
    FROM (
        SELECT
            ap.ApplicantId AS Id,
            ap.FullName,
            ap.Phone,
            ap.Email,
            p.PositionName AS Position,
			p.PositionCode,
            ap.InterviewDate1,
			LD_Result1.[Name] AS Result1,
            ap.InterviewDate2,
			LD_Result2.[Name] AS Result2,
            ap.JoinDate,
			LD_Status.[Name] AS [Status],
            ap.Note,
			LD_Source.[Name] AS [Source],
            ap.[Resume],
            ap.EducationBackground,
            ap.DOB,
			ap.IsOnTop,
			ap.Grade AS GradeId,
			ap.ApplyDate,
			ap.YearsOfExperience,
            ROW_NUMBER() OVER (ORDER BY ap.IsOnTop DESC, ap.CreatedDate DESC) AS RowNum
        FROM dbo.Applicant AS ap WITH (NOLOCK)
            INNER JOIN dbo.Position AS p WITH (NOLOCK) ON ap.PositionId = p.PositionId
		    LEFT JOIN dbo.LookupData AS LD_Result1 WITH (NOLOCK) ON CAST(ap.Result1 AS VARCHAR) = LD_Result1.[Value]
			    AND LD_Result1.LookupId = (SELECT Id FROM dbo.Lookup WHERE Code = 'AR')
		    LEFT JOIN dbo.LookupData AS LD_Result2 WITH (NOLOCK) ON CAST(ap.Result1 AS VARCHAR) = LD_Result2.[Value]
			    AND LD_Result2.LookupId = (SELECT Id FROM dbo.Lookup WHERE Code = 'AR')
		    LEFT JOIN dbo.LookupData AS LD_Status WITH (NOLOCK) ON CAST(ap.[Status] AS VARCHAR) = LD_Status.[Value]
			    AND LD_Status.LookupId = (SELECT Id FROM dbo.Lookup WHERE Code = 'CVS')
		    LEFT JOIN dbo.LookupData AS LD_Source WITH (NOLOCK) ON CAST(ap.[Source] AS VARCHAR) = LD_Source.[Value]
			AND LD_Source.LookupId = (SELECT Id FROM dbo.Lookup WHERE Code = 'AS')
        WHERE ap.FullName LIKE '%' + @FullName + '%' AND
			DATEDIFF(DAY, ISNULL(@FromDate, '01-01-1900'), ap.CreatedDate) >= 0 AND
			DATEDIFF(DAY, ISNULL(@ToDate, dbo.fnc_GetClientDateTime()), ap.CreatedDate) <= 0 AND
            (@Status = 0 OR ap.Status = @Status) AND
            (@PositionId = 0 OR ap.PositionId = @PositionId)
    ) AS PaginatedData
    WHERE RowNum BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND @PageNumber * @PageSize;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetById]
    @ApplicantId INT
AS
/*
Created: 20231123@Lilac.Nguyen
Task: Get by ApplicantId.
DB: NexHRM
Original: 

Revisions:  
	- 20250310@Henry.Dinh: add management grade
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT ap.ApplicantId AS Id
	    ,ap.FullName
	    ,ap.PositionId
	    ,ap.Phone
	    ,ap.Email
	    ,ap.ApplyDate
	    ,ap.YearsOfExperience
	    ,ap.DOB
	    ,ap.ProfessionalLevel
	    ,ap.Major AS ProfessionalMajor
	    ,ap.[Status]
	    ,ap.[Resume]
	    ,ap.InterviewDate1
	    ,ap.Result1
	    ,ap.InterviewDate2
	    ,ap.Result2
	    ,ap.JoinDate
	    ,ap.[Source]
	    ,ap.RecruiterId
	    ,e.NickName AS NickNameRecruiter 
	    ,e.FullName AS FullNameRecruiter
	    ,ap.EducationBackground
	    ,ap.Grade
	    ,ap.ManagementGrade
	    ,ap.InterviewerIds AS InterviewerIdString
	    ,ap.InterviewerComment
	    ,ap.Note
	    ,ap.FinalComment
	    ,ap.OfferSalary
	    ,ap.ExpectedSalary
    FROM dbo.Applicant AS ap WITH (NOLOCK)
	    LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ap.RecruiterId
    WHERE ap.ApplicantId = @ApplicantId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetCVStatus]
AS
/*
Created: 20240327@Lilac.Nguyen
Task: Get CV Status.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
	    ld.[Name]
	    ,CAST(ld.[Value] AS INT) AS [Value]
	FROM dbo.LookupData AS ld WITH (NOLOCK)
	    LEFT JOIN dbo.Lookup AS lu WITH (NOLOCK) ON lu.Id = ld.LookupId
	WHERE lu.Code = 'CVS'	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetInterViewer]
AS
/*
Created: 20240819@Quinn.Vo
Task: Get interviewer .
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
   SET NOCOUNT ON;

    SELECT Id
        ,NickName
    FROM dbo.Employee WITH (NOLOCK) WHERE IsDeleted = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetProfessionalLevel]
AS
/*
Created: 20240327@Lilac.Nguyen
Task: Get Professional Level.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ld.[Name]
	    ,CAST(ld.[Value] AS INT) AS [Value]
	FROM dbo.LookupData AS ld WITH (NOLOCK)
	    LEFT JOIN dbo.Lookup AS lu WITH (NOLOCK) ON lu.Id = ld.LookupId
	WHERE lu.Code = 'APL'	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetResult]
AS
/*
Created: 20240327@Lilac.Nguyen
Task: Get Result.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ld.[Name]
	    ,CAST(ld.[Value] AS INT) AS [Value]
	FROM dbo.LookupData AS ld WITH (NOLOCK)
	    LEFT JOIN dbo.Lookup AS lu WITH (NOLOCK) ON lu.Id = ld.LookupId
	WHERE lu.Code = 'AR'	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_GetSource]
AS
/*
Created: 20240327@Lilac.Nguyen
Task: Get Source.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ld.[Name]
	    ,CAST(ld.[Value] AS INT) AS [Value]
	FROM dbo.LookupData AS ld WITH (NOLOCK)
	    LEFT JOIN dbo.Lookup AS lu WITH (NOLOCK) ON lu.Id = ld.LookupId
	WHERE lu.Code = 'AS'	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_Insert] 
    @FullName NVARCHAR(100)
    ,@PositionId INT
    ,@Phone VARCHAR(20) = NULL
	,@Email VARCHAR(50) = NULL
    ,@DOB DATETIME = NULL
	,@ProfessionalMajor NVARCHAR(100) = NULL
	,@ProfessionalLevel TINYINT = NULL
	,@ApplyDate DATETIME = NULL
	,@YearsOfExperience FLOAT = NULL
	,@Resume VARCHAR(MAX) = NULL
	,@Status TINYINT = NULL
	,@InterviewDate1 DATETIME = NULL
	,@Result1 TINYINT = NULL
	,@InterviewDate2 DATETIME = NULL
	,@Result2 TINYINT = NULL
	,@JoinDate DATETIME = NULL
	,@Source TINYINT = NULL
    ,@RecruiterId INT = NULL
	,@EducationBackground NVARCHAR(500) = NULL
	,@Grade TINYINT = NULL
	,@ManagementGrade TINYINT = NULL
	,@InterviewerComment NVARCHAR(MAX) = NULL
	,@Note NVARCHAR(MAX) = NULL
	,@InterviewerIds VARCHAR(255) = NULL
	,@OfferSalary NVARCHAR(MAX) = NULL
	,@ExpectedSalary NVARCHAR(MAX) = NULL
	,@FinalComment NVARCHAR(MAX) = NULL
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20231121@Lilac.Nguyen
Task: Insert Applicant.
DB: NexHRM
Original: 

Revisions:  
	- 20250310@Henry.Dinh: add management grade
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Applicant(
        [FullName]
		,[PositionId]
		,[Phone]
		,[Email]
		,[DOB]
		,[Major]
		,[ProfessionalLevel]
		,[ApplyDate]
		,[YearsOfExperience]
		,[Resume]
		,[Status]
		,[InterviewDate1]
		,[Result1]
		,[InterviewDate2]
		,[Result2]
		,[JoinDate]
		,[Source]
		,[RecruiterId]
		,[EducationBackground]
		,[Grade]
		,[ManagementGrade]
		,[InterviewerComment]
		,[Note]
		,[InterviewerIds]
		,[OfferSalary]
		,[ExpectedSalary]
		,[FinalComment]
		,[CreatedBy]
		,[CreatedDate]
		,[IsOnTop]
        )
    VALUES (
        @FullName
		,@PositionId
		,@Phone
		,@Email
		,@DOB
		,@ProfessionalMajor
		,@ProfessionalLevel
		,@ApplyDate
		,@YearsOfExperience
		,@Resume
		,@Status
		,@InterviewDate1
		,@Result1
		,@InterviewDate2
		,@Result2
		,@JoinDate
		,@Source
		,@RecruiterId
		,@EducationBackground
		,@Grade
		,@ManagementGrade
		,@InterviewerComment
		,@Note
		,@InterviewerIds
		,@OfferSalary
		,@ExpectedSalary
		,@FinalComment
		,@CreatedBy
		,dbo.fnc_GetClientDateTime()
		,0
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_Update] 
    @Id INT
    ,@FullName NVARCHAR(100)
    ,@PositionId INT
    ,@Phone VARCHAR(20) = NULL
	,@Email VARCHAR(50) = NULL
    ,@DOB DATETIME = NULL
	,@ProfessionalMajor NVARCHAR(100) = NULL
	,@ProfessionalLevel TINYINT = NULL
	,@ApplyDate DATETIME = NULL
	,@YearsOfExperience FLOAT = NULL
	,@Resume VARCHAR(MAX) = NULL
	,@Status TINYINT = NULL
	,@InterviewDate1 DATETIME = NULL
	,@Result1 TINYINT = NULL
	,@InterviewDate2 DATETIME = NULL
	,@Result2 TINYINT = NULL
	,@JoinDate DATETIME = NULL
	,@Source TINYINT = NULL
    ,@RecruiterId INT = NULL
	,@EducationBackground NVARCHAR(500) = NULL
	,@Grade TINYINT = NULL
	,@ManagementGrade TINYINT = NULL
	,@InterviewerComment NVARCHAR(MAX) = NULL
	,@Note NVARCHAR(MAX) = NULL
	,@InterviewerIds VARCHAR(255) = NULL
	,@OfferSalary NVARCHAR(MAX) = NULL
	,@ExpectedSalary NVARCHAR(MAX) = NULL
	,@FinalComment NVARCHAR(MAX) = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20231127@Lilac.Nguyen
Task: Update Applicant.
DB: NexHRM
Original: 

Revisions:  
	- 20250310@Henry.Dinh: add management grade
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Applicant WITH (ROWLOCK, UPDLOCK)
    SET 
         [FullName] = @FullName
		,[PositionId] = @PositionId
		,[Phone] = @Phone
		,[Email] = @Email
		,[DOB] = @DOB
		,[Major] = @ProfessionalMajor
		,[ProfessionalLevel] = @ProfessionalLevel
		,[ApplyDate] = @ApplyDate
		,[YearsOfExperience] = @YearsOfExperience
		,[Resume] = @Resume
		,[Status] = @Status
		,[InterviewDate1] = @InterviewDate1
		,[Result1] = @Result1
		,[InterviewDate2] = @InterviewDate2
		,[Result2] = @Result2
		,[JoinDate]	= @JoinDate
		,[Source] = @Source
		,[RecruiterId] = @RecruiterId
		,[EducationBackground] = @EducationBackground
		,[Grade] = @Grade
		,[ManagementGrade] = @ManagementGrade
		,[InterviewerComment] = @InterviewerComment
		,[Note] = @Note
		,[InterviewerIds] = @InterviewerIds
		,[OfferSalary] = @OfferSalary
		,[ExpectedSalary] = @ExpectedSalary
		,[FinalComment]	= @FinalComment
    WHERE ApplicantId = @Id

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update applicant failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_UpdateOnTop] 
    @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240328@Lilac.Nguyen
Task: Update On Top.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

	UPDATE dbo.Applicant WITH (UPDLOCK, ROWLOCK)
	SET IsOnTop = CASE WHEN ISNULL(IsOnTop, 0) = 0 THEN 1 ELSE 0 END
	WHERE ApplicantId = @Id;

	SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Applicant_UpdateSalary] 
	@ApplicantId INT
	,@OfferSalary VARCHAR(500)
	,@ExpectedSalary VARCHAR(500)
	,@FinalComment VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
BEGIN
    UPDATE dbo.Applicant WITH (ROWLOCK, UPDLOCK)
	SET OfferSalary = @OfferSalary
		,ExpectedSalary = @ExpectedSalary
		,FinalComment = @FinalComment
		,CreatedDate = dbo.fnc_GetClientDateTime()
	WHERE ApplicantId = @ApplicantId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Conversation_Get]
	@EmployeeId INT
AS
/*
Created: 2019@elton.ton
Task: Get group conversations
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT c.ConversationId
		,c.ConversationName
	FROM HRM.dbo.Conversation c WITH (NOLOCK)
	WHERE c.IsActive = 1
		AND (
            c.CreatedBy = @EmployeeId 
			OR EXISTS (
				SELECT 1
				FROM HRM.dbo.ConversationPermission cp WITH (NOLOCK)
				WHERE c.ConversationId = cp.ConversationId
					AND cp.EmployeeId = @EmployeeId
			)
		)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Conversation_GetByCreator]
	@CreatedBy INT
AS
/*
Created: 20190412@elton.ton
Task: Get conversations
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT c.ConversationId
		,c.SkypeConversationId
		,c.ConversationName
		,c.IsActive
	FROM HRM.dbo.Conversation c WITH (NOLOCK)
	WHERE c.CreatedBy = @CreatedBy

	SELECT cp.EmployeeId
		,e.NickName
		,cp.ConversationId
	FROM HRM.dbo.ConversationPermission cp WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON cp.EmployeeId = e.Id
	WHERE EXISTS (
		SELECT 1
		FROM HRM.dbo.Conversation c WITH (NOLOCK)
		WHERE c.CreatedBy = @CreatedBy AND c.ConversationId = cp.ConversationId
	)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Conversation_GetById]
	@ConversationId INT
AS
/*
Created: 20190412@elton.ton
Task: Get conversations
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT c.ConversationId
		,c.SkypeConversationId
		,c.ConversationName
		,c.IsActive
	FROM HRM.dbo.Conversation c WITH (NOLOCK)
	WHERE c.ConversationId = @ConversationId

	SELECT cp.EmployeeId
		,e.NickName
		,cp.ConversationId
	FROM HRM.dbo.ConversationPermission cp WITH (NOLOCK)
		INNER JOIN HRM.dbo.Employee e WITH (NOLOCK) ON cp.EmployeeId = e.Id
	WHERE cp.ConversationId = @ConversationId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Conversation_Insert]
	@SkypeConversationId VARCHAR(255)
	,@ConversationName NVARCHAR(255)
	,@CreatedBy INT
	,@PermittedEmployeeIds VARCHAR(MAX) = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190409@Elton.ton
Task: create new conversation
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM dbo.Conversation c WITH (NOLOCK) WHERE c.SkypeConversationId = @SkypeConversationId)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The conversation is already existed.'
		RETURN
	END

	INSERT INTO [dbo].[Conversation]
           ([SkypeConversationId]
           ,[ConversationName]
           ,[CreatedBy])
     VALUES
           (@SkypeConversationId
           ,@ConversationName
           ,@CreatedBy)

	IF @PermittedEmployeeIds IS NOT NULL
	BEGIN
		DECLARE @Id INT = SCOPE_IDENTITY()
		INSERT INTO [dbo].[ConversationPermission]
           ([ConversationId]
           ,[EmployeeId])
		SELECT @Id
			,val
		FROM dbo.fnc_strSplit(@PermittedEmployeeIds, ',')
	END
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Conversation_Update]
	@ConversationId INT
	,@SkypeConversationId VARCHAR(255)
	,@ConversationName NVARCHAR(255)
	,@PermittedEmployeeIds VARCHAR(MAX) = NULL
	,@EditedBy INT
	,@IsActive BIT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190409@Elton.ton
Task: update conversation
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM dbo.Conversation c WITH (NOLOCK) 
		WHERE c.SkypeConversationId = @SkypeConversationId AND c.ConversationId <> @ConversationId)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The conversation is already existed.'
		RETURN
	END

	IF NOT EXISTS (SELECT 1 FROM dbo.Conversation c WITH (NOLOCK) 
		WHERE c.ConversationId = @ConversationId AND c.CreatedBy = @EditedBy)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You dont have permission to edit this conversation.'
		RETURN
	END

	UPDATE dbo.Conversation WITH (ROWLOCK, UPDLOCK)
	SET SkypeConversationId = @SkypeConversationId
		,ConversationName = @ConversationName
		,IsActive = @IsActive
	WHERE ConversationId = @ConversationId

	SELECT val AS EmployeeId
	INTO #permittedEmployee
	FROM dbo.fnc_strSplit(@PermittedEmployeeIds, ',')

	INSERT INTO [dbo].[ConversationPermission]
           ([ConversationId]
           ,[EmployeeId])
	SELECT @ConversationId
		,pe.EmployeeId
	FROM #permittedEmployee pe
	WHERE NOT EXISTS (
		SELECT 1
		FROM dbo.ConversationPermission cp WITH (NOLOCK)
		WHERE cp.ConversationId = @ConversationId AND cp.EmployeeId = pe.EmployeeId
	)

	DELETE cp
	FROM dbo.ConversationPermission cp
	WHERE ConversationId = @ConversationId
		AND NOT EXISTS (
			SELECT 1
			FROM #permittedEmployee pe
			WHERE cp.EmployeeId = pe.EmployeeId
		)

	DROP TABLE #permittedEmployee
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_MappingV2]
    @ConversationId VARCHAR(255),
    @UserName VARCHAR(500),
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
    Created: 20250613@Henry.Dinh
    Task: update conversation id for employee
    DB: NexHRM, LumiHRM, NexVisionHRM
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
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeJobGrade_Insert] 
    @EmpId INT
    ,@JobGradeId INT
    ,@OldJobGradeId INT
    ,@IsPromotion BIT
    ,@EffectiveDate DATETIME
    ,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240104@Lilac.Nguyen
Task: Insert Promotion
DB: NexHRM
Original: 
Revisions:  
    20252602@Henry.Dinh: Set others IsCurrentUsed to 0
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	UPDATE dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
	SET IsCurrentUsed = 0
	WHERE EmpId = @EmpId AND (IsDeleted = 0 OR IsDeleted IS NULL);

    IF EXISTS (
        SELECT 1
        FROM dbo.EmployeeJobGrade WITH (NOLOCK)
        WHERE EmpId = @EmpId 
        AND JobGradeId = @JobGradeId
        AND (IsDeleted = 0 OR IsDeleted IS NULL)
        AND (AffectedDate = @EffectiveDate)
    )
    BEGIN
        UPDATE dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
        SET 
            IsPromotion = @IsPromotion,
            Note = @Note,
            OldJobGradeId = @OldJobGradeId,
			IsCurrentUsed = 1,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE 
            EmpId = @EmpId 
            AND JobGradeId = @JobGradeId
            AND (IsDeleted = 0 OR IsDeleted IS NULL)
            AND (AffectedDate = @EffectiveDate)
        SELECT @ErrCode = 0, @ErrMsg = ''
        RETURN
    END
    
    ELSE
	BEGIN
		INSERT INTO dbo.EmployeeJobGrade(
			EmpId
			,JobGradeId
			,OldJobGradeId
			,AffectedDate
			,ModifiedBy
			,ModifiedDate
			,IsCurrentUsed
			,Note
			,IsDeleted
			,IsPromotion
		)
		VALUES (
			@EmpId
			,@JobGradeId
			,@OldJobGradeId
			,@EffectiveDate
			,@ModifiedBy
			,dbo.fnc_GetClientDateTime()
			,1 
			,@Note
			,NULL
			,@IsPromotion
		);
	END
    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeManagementGrade_Insert] 
    @EmpId INT
    ,@ManagementGradeId INT
    ,@OldManagementGradeId INT
    ,@IsPromotion BIT
    ,@EffectiveDate DATETIME
    ,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20252602@Henry.Dinh
Task: Insert Management Grade
DB: NexHRM
Original: 
Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	UPDATE dbo.EmployeeManagementGrade WITH (ROWLOCK, UPDLOCK)
	SET IsCurrentUsed = 0
	WHERE EmpId = @EmpId AND (IsDeleted = 0 OR IsDeleted IS NULL);

    IF EXISTS (
        SELECT 1
        FROM dbo.EmployeeManagementGrade WITH (NOLOCK)
        WHERE EmpId = @EmpId 
        AND ManagementGradeId = @ManagementGradeId
        AND (IsDeleted = 0 OR IsDeleted IS NULL)
        AND (AffectedDate = @EffectiveDate)
    )
    BEGIN
        UPDATE dbo.EmployeeManagementGrade WITH (ROWLOCK, UPDLOCK)
        SET 
            IsPromotion = @IsPromotion,
            Note = @Note,
            OldManagementGradeId = @OldManagementGradeId,
			IsCurrentUsed = 1,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE 
            EmpId = @EmpId 
            AND ManagementGradeId = @ManagementGradeId
            AND (IsDeleted = 0 OR IsDeleted IS NULL)
            AND (AffectedDate = @EffectiveDate)
        SELECT @ErrCode = 0, @ErrMsg = ''
        RETURN
    END
    
    ELSE
	BEGIN
		INSERT INTO dbo.EmployeeManagementGrade(
			EmpId
			,ManagementGradeId
			,OldManagementGradeId
			,AffectedDate
			,ModifiedBy
			,ModifiedDate
			,IsCurrentUsed
			,Note
			,IsDeleted
			,IsPromotion
		)
		VALUES (
			@EmpId
			,@ManagementGradeId
			,@OldManagementGradeId
			,@EffectiveDate
			,@ModifiedBy
			,dbo.fnc_GetClientDateTime()
			,1 
			,@Note
			,NULL
			,@IsPromotion
		);
	END
    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[JobGrade_GetAll]
AS
/*
Created: 20240104@Lilac.Nguyen
Task: Get jobGrade.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT JobGradeId AS Id
	,Code
	,Name
	FROM dbo.JobGrade WITH (NOLOCK)
    WHERE (IsDeleted = 0 OR IsDeleted IS NULL)  
END
GO

CREATE OR ALTER PROCEDURE [dbo].[JobGrade_Insert] 
    @EmpId INT
    ,@JobGradeId INT
	,@EffectiveDate DATETIME
	,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240102@Lilac.Nguyen
Task: Insert JobGrade
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.EmployeeJobGrade(
		EmpId
		,JobGradeId
		,OldJobGradeId
		,AffectedDate
		,ModifiedBy
		,ModifiedDate
		,IsCurrentUsed
		,Note
		,IsDeleted
		,IsPromotion
        )

    VALUES (
        @EmpId
		,@JobGradeId
		,@JobGradeId
		,@EffectiveDate
		,@ModifiedBy
		,dbo.fnc_GetClientDateTime()
		,1
		,@Note
		,NULL
		,0
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[JobTitle_Delete] @JobTitleId INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20240731@Quinn.Vo
Task: delete jobtitle.
DB: HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM Employee e WITH (NOLOCK)
                WHERE e.JobTitleId = @JobTitleId
                )
            )
    BEGIN
        SELECT @ErrCode = 100
            ,@ErrMsg = 'The employee is not empty so cannot delete it'

        RETURN
    END

    DELETE FROM dbo.JobTitle
    WHERE JobTitleId = @JobTitleId

    SET @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[JobTitle_Insert]
    @Name NVARCHAR(100)
	,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN

    SET NOCOUNT ON;

    IF (EXISTS(SELECT 1 FROM dbo.JobTitle WITH (NOLOCK) WHERE [Name]=@Name))
    BEGIN
        SELECT @ErrCode=100,
                @ErrMsg = 'The job title already exist'
        RETURN
    END

    INSERT INTO dbo.JobTitle(Name, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
    VALUES(@Name, @CreatedBy, dbo.fnc_GetClientDateTime(), @CreatedBy, dbo.fnc_GetClientDateTime())

    SET @ErrCode=0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[JobTitle_Update]
    @JobTitleId INT,
    @Name NVARCHAR(100),
    @ErrMsg NVARCHAR(500) = '',
    @ErrCode INT = NULL
AS
BEGIN

    SET NOCOUNT ON;

    IF (NOT EXISTS(SELECT 1 FROM dbo.JobTitle WITH (NOLOCK)
            WHERE JobTitleId = @JobTitleId))
    BEGIN
        SELECT @ErrCode = 100,
                @ErrMsg = 'The job title does not exist'
        RETURN
    END
    
	UPDATE dbo.JobTitle WITH (UPDLOCK, ROWLOCK)
	SET Name = @Name
    WHERE JobTitleId = @JobTitleId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Log_WriteLog]
    @LogActionID int,
    @LogDate datetime,
    @Description NVARCHAR(4000),
    @TrackingValue NVARCHAR(MAX),
    @UserID int
AS
/*
Created: 2012/04/20@Dan
Task: write log
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

INSERT INTO [dbo].[Log]
           ([LogActionID]
           ,[LogDate]
           ,[Description]
           ,[TrackingValue]
           ,[UserID])
     VALUES
           (
            @LogActionID
           ,@LogDate
           ,@Description
           ,@TrackingValue
           ,@UserID
            )
GO

CREATE OR ALTER PROCEDURE [dbo].[Lookup_GetLookupData]
    @lookupCode varchar(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @lookupId int
    SELECT @lookupId = Id FROM dbo.[Lookup] WITH (NOLOCK) WHERE Code = @lookupCode
    
    SELECT * FROM dbo.LookupData WITH (NOLOCK) WHERE LookupId = @lookupId
    ORDER BY [Name]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ManagementGrade_GetAll]
AS
/*
Created: 20252602@Henry.Dinh
Task: Get Management Grade
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT ManagementGradeId AS Id
	,Code
	,Name
	FROM dbo.ManagementGrade WITH (NOLOCK)
    WHERE (IsDeleted = 0 OR IsDeleted IS NULL)  
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Position_Delete] @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 202311270@Lilac.Nguyen
Task: delete a position 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF (
            NOT EXISTS (
                SELECT 1
                FROM dbo.Position WITH (NOLOCK)
                WHERE PositionId = @Id
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The position does not exists '

        RETURN
    END
    
    IF (
        EXISTS (
            SELECT 1
            FROM dbo.Position AS p WITH (NOLOCK)
            INNER JOIN dbo.Applicant AS a WITH (NOLOCK) ON p.PositionId = a.PositionId
            WHERE p.PositionId = @Id
        )
    )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The position has applicants '

        RETURN
    END
    
    DELETE FROM dbo.Position
    WHERE PositionId = @Id

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Position_GetAll] @Keyword NVARCHAR(100) = ''
AS
/*
Created: 20231116@Lilac.Nguyen
Task: Get all position.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT PositionId
	,PositionCode
	,PositionName
	,DisplayOrder
	,IsActive
	FROM dbo.Position WITH (NOLOCK)
    WHERE  PositionName LIKE '%' + @Keyword + '%' OR PositionCode LIKE '%' + @Keyword + '%' 
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Position_GetById]
    @PositionId INT
AS
/*
Created: 20231127@Lilac.Nguyen
Task: Get by Position.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT po.PositionId
	,po.PositionName
	,po.PositionCode
	,po.DisplayOrder
	,po.IsActive
    FROM dbo.Position AS po WITH (NOLOCK)
    WHERE po.PositionId = @PositionId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Position_Insert] 
    @Code VARCHAR(20)
    ,@Name NVARCHAR(100)
    ,@DisplayOrder INT = NULL
    ,@IsActive BIT = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20231127@Lilac.Nguyen
Task: Insert Position.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Position WITH (NOLOCK)
                WHERE PositionCode = @Code OR PositionName = @Name
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Position code or name has already exists'

        RETURN
    END

    INSERT INTO dbo.Position(
        [PositionCode]
        ,[PositionName]
        ,[DisplayOrder]
        ,[IsActive]
        ,[ModifiedBy]
        ,[ModifiedDate]
        )
    VALUES (
        @Code
        ,@Name
        ,@DisplayOrder
        ,1
        ,@ModifiedBy
        ,dbo.fnc_GetClientDateTime()
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[RepeatType_GetAll]
AS
/*
Created: 
Task: Get Repeat type.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [Value] AS Id, [Name] AS [Description]
	FROM dbo.LookupData WITH (NOLOCK)
	WHERE LookupId = 5
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Delete]
	@ScheduledMessageId INT
	 ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: delete message by Id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DELETE
	FROM dbo.ScheduledMessage
	WHERE ScheduledMessageId = @ScheduledMessageId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Get]
	@YEAR INT
	,@CreatedBy INT
AS
/*
Created: 20190325@Elton.ton
Task: Get messages by year
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.ScheduledMessageId
		,sm.ConversationId
		,c.ConversationName
		,sm.SendDate
		,sm.Content
		,sm.CreatedBy
		,e.NickName
		,sm.IsSent
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON sm.CreatedBy = e.Id
		INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
	WHERE YEAR(sm.SendDate) = @YEAR AND sm.CreatedBy = @CreatedBy
	ORDER BY sm.SendDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_GetById]
	@ScheduledMessageId INT
AS
/*
Created: 20190325@Elton.ton
Task: Get message by Id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.ScheduledMessageId
		,sm.ConversationId
		,c.ConversationName
		,sm.SendDate
		,sm.Content
		,sm.CreatedBy
		,e.NickName
		,sm.IsSent
		,sm.StepNumber
		,sm.RepeatTypeID
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON sm.CreatedBy = e.Id
		INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
	WHERE sm.ScheduledMessageId = @ScheduledMessageId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_GetMessagesToProcess]
    @FromDate DATETIME
    ,@ToDate DATETIME
AS
/*
Created: 20190325@Elton.ton
Task: Get message by SendDate
DB: NexHRM
Original:
    - 20250507@Henry: Update hardcode conversationId
Revisions:
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SELECT sm.ScheduledMessageId
        ,sm.ConversationId
        ,sm.Content
        ,sm.CronExpression
        ,c.SkypeConversationId
    FROM dbo.ScheduledMessage sm WITH (NOLOCK)
        INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
    WHERE ((sm.SendDate BETWEEN @FromDate AND @ToDate
            AND sm.CronExpression IS NULL)
            OR (@ToDate >= sm.SendDate
            AND sm.CronExpression IS NOT NULL))
        AND sm.IsSent = 0
        AND CHARINDEX('B08J3UJ8WMA:' , c.SkypeConversationId ) = 1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Insert]
	@ConversationId INT
	,@SendDate DATETIME
	,@Content NVARCHAR(MAX)
	,@CreatedBy INT
	,@CronExpression VARCHAR(50) = null
	,@StepNumber INT = null
	,@RepeatTypeId INT = null
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: 
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	IF @RepeatTypeId IS NULL
    BEGIN
		INSERT INTO [dbo].[ScheduledMessage]
           ([ConversationId]
           ,[SendDate]
           ,[Content]
           ,[CreatedDate]
           ,[CreatedBy]
           )
		VALUES
           (@ConversationId
           ,@SendDate
           ,@Content
           ,dbo.fnc_GetClientDateTime()
           ,@CreatedBy
           )
    END
	ELSE
    BEGIN
		INSERT INTO [dbo].[ScheduledMessage]
           ([ConversationId]
           ,[SendDate]
           ,[Content]
           ,[CreatedDate]
           ,[CreatedBy]
		   ,[CronExpression]
		   ,[StepNumber]
		   ,[RepeatTypeID]
           )
		VALUES
           (@ConversationId
           ,@SendDate
           ,@Content
           ,dbo.fnc_GetClientDateTime()
           ,@CreatedBy
		   ,@CronExpression
		   ,@StepNumber
		   ,@RepeatTypeId
           )
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Update]
	@ScheduledMessageId INT
	,@ConversationId INT
	,@SendDate DATETIME
	,@Content NVARCHAR(MAX)
	,@CronExpression VARCHAR(50) = null
	,@StepNumber INT = null
	,@RepeatTypeId INT = null
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: update message
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	IF @RepeatTypeId IS NULL
    BEGIN
		UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
		SET [ConversationId] = @ConversationId
		  ,[SendDate] = @SendDate
		  ,[Content] = @Content
		  ,[CronExpression] = null
		  ,[StepNumber] = null
		  ,[RepeatTypeId] = null
		WHERE ScheduledMessageId = @ScheduledMessageId
    END
	ELSE
    BEGIN
		UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
		SET [ConversationId] = @ConversationId
		  ,[SendDate] = @SendDate
		  ,[Content] = @Content
		  ,[CronExpression] = @CronExpression
		  ,[StepNumber] = @StepNumber
		  ,[RepeatTypeId] = @RepeatTypeId
		WHERE ScheduledMessageId = @ScheduledMessageId
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_UpdateStatus]
	@ScheduledMessageIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: update selected messages are sent
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
	SET IsSent = 1
	WHERE EXISTS (
		SELECT 1
		FROM dbo.fnc_strSplit(@ScheduledMessageIds, ',')
		WHERE val = ScheduledMessageId
	)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TOEIC_Update] @empId INT
	,@listening INT = NULL
	,@reading INT = NULL
	,@lrExamDate DATETIME = NULL
	,@oldListening INT = NULL
	,@oldReading INT = NULL
	,@speaking INT = NULL
	,@writing INT = NULL
	,@MinimumScore INT = NULL
	,@swExamDate DATETIME = NULL
	,@oldSpeaking INT = NULL
	,@oldWriting INT = NULL
	,@inputDate DATETIME = NULL
	,@oldDeadlineDate DATETIME = NULL
	,@modifiedBy INT = NULL
	,@modifiedDate DATETIME = NULL
	,@createdBy INT = NULL
	,@createdDate DATETIME = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 
Task: update employee's TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @monthEvaluation1H TINYINT = 5,
@monthEvaluation2H TINYINT = 11,
@toDay DATETIME = dbo.fnc_GetClientDateTime()

BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.TOEIC WITH (NOLOCK)
			WHERE EmpId = @empId
			AND ( Listening + Reading ) >= @MinimumScore
			AND InputDate > @toDay
			AND (MONTH(InputDate) != @monthEvaluation1H OR MONTH(InputDate) != @monthEvaluation2H)
			AND YEAR(InputDate) != YEAR(@toDay)
			AND MONTH(@toDay) IN ( @monthEvaluation1H, @monthEvaluation2H)
			AND (MONTH(@lrExamDate) = @monthEvaluation1H OR MONTH(@lrExamDate) = @monthEvaluation2H)
			AND YEAR(@lrExamDate) = YEAR(@toDay)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Toeic score still effect. Please do not update scores this time.'

			RETURN
	END
	ELSE
	BEGIN
		IF EXISTS (
			SELECT 1
			FROM dbo.TOEIC WITH (NOLOCK)
			WHERE EmpId = @empId
			)
		BEGIN -- update
		    UPDATE dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
		    SET Listening = @listening
			    ,Reading = @reading
			    ,LRExamDate = @lrExamDate
			    ,OldListening = @oldListening
			    ,OldReading = @oldReading
			    ,Speaking = @speaking
			    ,Writing = @writing
			    ,SWExamDate = @swExamDate
			    ,OldSpeaking = @oldSpeaking
			    ,OldWriting = @oldWriting
			    ,MinimumScore = @MinimumScore
			    ,InputDate = @inputDate
			    ,ModifiedBy = @modifiedBy
			    ,ModifiedDate = @modifiedDate
		    WHERE EmpId = @empId;
	    END
	    ELSE
	    BEGIN -- insert
		    INSERT INTO dbo.TOEIC (
		        EmpId
		        ,Listening
		        ,Reading
		        ,LRExamDate
		        ,OldListening
		        ,OldReading
		        ,Speaking
		        ,Writing
		        ,SWExamDate
		        ,OldSpeaking
		        ,OldWriting
		        ,MinimumScore
		        ,InputDate
		        ,ModifiedBy
		        ,ModifiedDate
		        ,CreatedBy
		        ,CreatedDate
		        )
	        VALUES (
		        @empId
		        ,@listening
		        ,@reading
		        ,@lrExamDate
		        ,@oldListening
		        ,@oldReading
		        ,@speaking
		        ,@writing
		        ,@swExamDate
		        ,@oldSpeaking
		        ,@oldWriting
		        ,@MinimumScore
		        ,@inputDate
		        ,@modifiedBy
		        ,@modifiedDate
		        ,@createdBy
		        ,@createdDate
		        )
	    END
	END

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Toeic score failed.';
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_Delete] 
	@Id INT
	,@DeletedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230417@Brian.Nguyen
Task: Delete Training And Development.
DB: NexHRM
Original: 

Revisions:  
	- 20241202@Lilac: Add certificate upload
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
	
	DECLARE 
		@logData VARCHAR(MAX)
		,@employeeId INT

	SELECT @logData = '<TrainingAndDevelopment Id="'+ COALESCE(CAST(Id AS VARCHAR(50)),SPACE(1)) + '" ' +
		'EmpId="'+ COALESCE(CAST(EmpId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Publish="'+ COALESCE(Publish ,SPACE(1)) + '" ' +
		'ConductedBy="'+ COALESCE(ConductedBy,SPACE(1)) + '" ' +
		'Description="'+ COALESCE(Description,SPACE(1)) + '" ' +
		'Certificate="'+ COALESCE(Certificate,SPACE(1)) + '" ' +
		'TypeId="'+ COALESCE(CAST(TypeId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Name="'+ COALESCE(Name,SPACE(1)) + '" ' +
		'Organization="'+ COALESCE(Organization,SPACE(1)) + '" ' +
		'StartDate="'+ COALESCE(CONVERT(VARCHAR(300), StartDate),SPACE(1)) + '" ' +
		'EndDate="'+ COALESCE(CONVERT(VARCHAR(300), EndDate),SPACE(1)) + '" ' +
		'ExamDate="'+ COALESCE(CONVERT(VARCHAR(300), ExamDate),SPACE(1)) + '" ' +
		'ExpirationDate="'+ COALESCE(CONVERT(VARCHAR(300), ExpirationDate),SPACE(1)) + '" ' +
		'CreatedDate="'+ COALESCE(CONVERT(VARCHAR(300), CreatedDate),SPACE(1)) + '" ' +
		'ModifyDate="'+ COALESCE(CONVERT(VARCHAR(300), ModifyDate),SPACE(1)) + '" ' +
		'ModifyBy="'+ COALESCE(CAST(ModifyBy AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DeletedDate="'+ COALESCE(CAST(dbo.fnc_GetClientDateTime() AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DeletedBy="'+ COALESCE(CAST(@DeletedBy AS VARCHAR(50)),SPACE(1)) + '" ' +
		'CreatedBy="'+ COALESCE(CAST(CreatedBy AS VARCHAR(50)),SPACE(1)) +  '" />'
		,@employeeId = EmpId
	FROM dbo.[TrainingAndDevelopment] WITH (NOLOCK)
	WHERE Id = @Id

	DELETE FROM [dbo].[TrainingAndDevelopment]
	WHERE Id = @Id

	INSERT INTO dbo.[Log](LogActionID, LogDate, Description, TrackingValue, UserID)
	VALUES(7, dbo.fnc_GetClientDateTime(), 'Delete Training And Development', @logData, @employeeId)

    SELECT @ErrCode = 0, @ErrMsg = ''
END
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

CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_Update] 
	@Id INT
    ,@ConductedBy NVARCHAR(100) = NULL
    ,@Publish NVARCHAR(20)
    ,@TypeId Int
    ,@Name NVARCHAR(200)
    ,@Organization NVARCHAR(200)
	,@Description NVARCHAR(500) = NULL
    ,@StartDate DATETIME = NULL
    ,@EndDate DATETIME = NULL
    ,@ExamDate DATETIME = NULL
    ,@ExpirationDate DATETIME = NULL
    ,@ModifyBy INT
	,@Certificate VARCHAR(MAX) = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230417@Brian.Nguyen
Task: Update Training And Development
DB: NexHRM
Original: 

Revisions:  
- 20241202@Lilac: Add certificate upload
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE 
		@logData VARCHAR(MAX)
		,@employeeId INT

	SELECT @logData = '<TrainingAndDevelopment_Old Id="'+ COALESCE(CAST(Id AS VARCHAR(50)),SPACE(1)) + '" ' +
		'EmpId="'+ COALESCE(CAST(EmpId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Publish="'+ COALESCE(Publish ,SPACE(1)) + '" ' +
		'ConductedBy="'+ COALESCE(ConductedBy,SPACE(1)) + '" ' +
		'Description="'+ COALESCE(Description,SPACE(1)) + '" ' +
		'TypeId="'+ COALESCE(CAST(TypeId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Name="'+ COALESCE(Name,SPACE(1)) + '" ' +
		'Organization="'+ COALESCE(Organization,SPACE(1)) + '" ' +
		'StartDate="'+ COALESCE(CONVERT(VARCHAR(300), StartDate),SPACE(1)) + '" ' +
		'Certificate="'+ COALESCE(Certificate,SPACE(1)) + '" ' +
		'EndDate="'+ COALESCE(CONVERT(VARCHAR(300), EndDate),SPACE(1)) + '" ' +
		'ExamDate="'+ COALESCE(CONVERT(VARCHAR(300), ExamDate),SPACE(1)) + '" ' +
		'ExpirationDate="'+ COALESCE(CONVERT(VARCHAR(300), ExpirationDate),SPACE(1)) + '" ' +
		'CreatedDate="'+ COALESCE( CONVERT(VARCHAR(300), CreatedDate),SPACE(1)) + '" ' +
		'ModifyDate="'+ COALESCE(CONVERT(VARCHAR(300), ModifyDate),SPACE(1)) + '" ' +
		'ModifyBy="'+ COALESCE(CAST(ModifyBy AS VARCHAR(50)),SPACE(1)) + '" ' +
		'CreatedBy="'+ COALESCE(CAST(CreatedBy AS VARCHAR(50)),SPACE(1)) + '" />' +
		'<TrainingAndDevelopment_new Id="'+ COALESCE(CAST(@Id AS VARCHAR(50)),SPACE(1)) + '" ' +
		'EmpId="'+ COALESCE(CAST(EmpId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Publish="'+ COALESCE(@Publish ,SPACE(1)) + '" ' +
		'ConductedBy="'+ COALESCE(@ConductedBy,SPACE(1)) + '" ' +
		'Description="'+ COALESCE(@Description,SPACE(1)) + '" ' +
		'TypeId="'+ COALESCE(CAST(@TypeId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Name="'+ COALESCE(@Name,SPACE(1)) + '" ' +
		'Organization="'+ COALESCE(@Organization,SPACE(1)) + '" ' +
		'Certificate="'+ COALESCE(@Certificate,SPACE(1)) + '" ' +
		'StartDate="'+ COALESCE(CONVERT(VARCHAR(300), @StartDate),SPACE(1)) + '" ' +
		'EndDate="'+ COALESCE(CONVERT(VARCHAR(300), @EndDate),SPACE(1)) + '" ' +
		'ExamDate="'+ COALESCE(CONVERT(VARCHAR(300), @ExamDate),SPACE(1)) + '" ' +
		'ExpirationDate="'+ COALESCE(CONVERT(VARCHAR(300), @ExpirationDate),SPACE(1)) + '" ' +
		'CreatedDate="'+ COALESCE(CONVERT(VARCHAR(300), CreatedDate),SPACE(1)) + '" ' +
		'ModifyDate="'+ COALESCE(CONVERT(VARCHAR(300), ModifyDate),SPACE(1)) + '" ' +
		'ModifyBy="'+ COALESCE(CAST(@ModifyBy AS VARCHAR(50)),SPACE(1)) + '" ' +
		'CreatedBy="'+ COALESCE(CAST(CreatedBy AS VARCHAR(50)),SPACE(1)) +  '" />'
		,@employeeId = EmpId
		FROM dbo.[TrainingAndDevelopment] WITH (NOLOCK)
		WHERE Id = @Id

	UPDATE [dbo].[TrainingAndDevelopment] WITH (ROWLOCK, UPDLOCK)
	   SET [ConductedBy] = @ConductedBy
		  ,[TypeId] = @TypeId
		  ,[Name] = @Name
		  ,[Organization] = @Organization
		  ,[StartDate] = @StartDate
		  ,[EndDate] = @EndDate
		  ,[ExamDate] = @ExamDate
		  ,[ExpirationDate] = @ExpirationDate
		  ,[ModifyDate] = dbo.fnc_GetClientDateTime()
		  ,[ModifyBy] = @ModifyBy
		  ,[Description] = @Description
		  ,[Publish] = @Publish
		  ,[Certificate] = @Certificate
	 WHERE Id = @Id

	INSERT INTO dbo.[Log](LogActionID, LogDate, Description, TrackingValue, UserID)
	VALUES(6, dbo.fnc_GetClientDateTime(), 'Update Training And Development', @logData, @employeeId)

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopmentType_Get]
AS
/*
Created: 20230417@Brian.Nguyen
Task: Get training and development type
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [Id]
      ,[Name]
	FROM [dbo].[TrainingAndDevelopmentType] WITH (NOLOCK)
END
GO