USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TimeRecord_ImportData]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_ImportData]
	@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160927
Task: Import data from BioAdmin to HRSmart system
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
• @importdate: the date to import data
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @FromDate DATETIME
		,@Today DATETIME
		,@TempDate DATETIME

	SELECT @Today = GETDATE()

	SELECT @FromDate = DATEADD(dd, - 1, @Today)

	EXEC dbo.TimeRecord_ImportFromBioStar @FromDate
		,@Today

	SELECT @TempDate = DATEADD(MM, 2, GETDATE())

	IF (DATEDIFF(MI, CAST(@Today AS DATE), @Today) < 5)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.Employee AS e WITH (NOLOCK)
					LEFT JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
					AND DATEDIFF(DD, WorkDate, @TempDate) = 0
					AND ISNULL(e.Enabled, 0) = 1
					AND ISNULL(e.isDeleted, 0) = 0
				)
		BEGIN
			EXEC dbo.EmployeeCalendar_Insert NULL
				,@TempDate
				,@TempDate
		END
	END

	EXEC [dbo].[EmployeeConversation_Mapping]
	-- EXEC dbo.TimeRecord_ImportBioStationTimeChange

	
	-- EXEC [LumiHRM].dbo.TimeRecord_ImportData
END
GO
