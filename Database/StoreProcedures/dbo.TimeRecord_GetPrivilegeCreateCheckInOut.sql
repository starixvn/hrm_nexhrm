USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TimeRecord_GetPrivilegeCreateCheckInOut]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_GetPrivilegeCreateCheckInOut] @EmpId INT
AS
/*
Created: 20211213@Teddy.le
Task: Get Privilege Create Check In/Out Record.
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsPrivilegeCreateCheckInOut BIT = 0,
		@Today DATETIME
		
	SET @Today = CONVERT(date, GETDATE())

	IF (EXISTS(SELECT 1 FROM dbo.WorkFromHomeRequest WITH (NOLOCK) 
		WHERE EmpId = @EmpId 
			AND @Today BETWEEN FromDate AND ToDate
			AND Status <> 3
		))
	BEGIN
		SET @IsPrivilegeCreateCheckInOut = 1
	END

	SELECT @IsPrivilegeCreateCheckInOut
END
GO
