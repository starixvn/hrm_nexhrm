USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Overtime_GetConversions]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_GetConversions]
	@fromDate		DATE
	,@toDate		DATE
    ,@employeeId    INT				= NULL
	,@GroupId		INT				= NULL
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
Created: 
Task: 
DB: NexHRM
Original: 

Revisions:  20200723@Dwight.Tran - Enhance NEX request
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
		otc.[Id]
		,otc.[Deduct] / 60.0	AS Deduct
		,otc.[DeductDate]
		,otc.[Description]
		,otc.[ModifiedDate]
		,otc.[EmpId] AS EmployeeId
		,e.NickName
		,e.FullName
		,e.GroupId
		,g.Code AS TeamCode
		,g.Name	AS TeamName
	FROM [dbo].[OvertimeConversion] AS otc WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON otc.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN dbo.Employee AS modi WITH (NOLOCK) ON otc.ModifiedBy = modi.Id
	WHERE DATEDIFF(DAY, @fromDate, otc.[DeductDate]) >= 0
		AND DATEDIFF(DAY, otc.[DeductDate], @toDate) >= 0
		AND (
			@employeeId IS NULL
			OR otc.[EmpId] = @employeeId
		)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)
END;
GO
