USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[SyncData_GetEmployeeByLastModifiedDate]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[SyncData_GetEmployeeByLastModifiedDate]
	@LastModifiedDate DATETIME
AS
/*
Created: 20201126@Oliver.Dang
Task: Get Employee to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT e.Id as SourceEmpId
		  ,e.DomainUsername
		  ,e.Code
		  ,e.NickName
		  ,e.FullName
		  ,e.JobTitleId
		  ,e.GroupId
		  ,e.[Enabled]
		  ,e.isDeleted
		  ,e.IsSpecialUser
		  ,e.Email
		  ,e.JoinedDate
		  ,e.ConfirmedDate
		  ,e.ResignedDate
		  ,e.ModifiedDate 
	FROM dbo.Employee e WITH (NOLOCK)
	WHERE e.ModifiedDate > @LastModifiedDate

END


GO
