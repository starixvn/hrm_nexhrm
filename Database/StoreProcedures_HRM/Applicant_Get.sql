USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Applicant_Get]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
    WHERE
        ap.FullName LIKE '%' + @FullName + '%' AND
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

        WHERE
            ap.FullName LIKE '%' + @FullName + '%' AND
			DATEDIFF(DAY, ISNULL(@FromDate, '01-01-1900'), ap.CreatedDate) >= 0 AND
			DATEDIFF(DAY, ISNULL(@ToDate, dbo.fnc_GetClientDateTime()), ap.CreatedDate) <= 0 AND
            (@Status = 0 OR ap.Status = @Status) AND
            (@PositionId = 0 OR ap.PositionId = @PositionId)
    ) AS PaginatedData
    WHERE RowNum BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND @PageNumber * @PageSize;
END
GO
