CREATE PROCEDURE [dbo].[Overtime_GetConversions_138194]
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
		,otc.[EmpId]
		,e.NickName
		,e.FullName
		,e.GroupId
		,g.Code
		,g.Name	
		,otc.ModifiedBy AS ModifiedById
		,modi.NickName
		,modi.FullName
	FROM [dbo].[OvertimeConversion_138194] AS otc WITH (NOLOCK)
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