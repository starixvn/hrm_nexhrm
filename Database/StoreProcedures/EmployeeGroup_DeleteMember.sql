USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeGroup_DeleteMember]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_DeleteMember]
	@EmpId INT
   ,@GroupId INT   
AS
/*
Created: 2012/06/11@Dan
Task: delete member from group (not allow delete maingroup)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN	
	DELETE dbo.EmployeeGroup_Employee	
	WHERE EmpID=@EmpID AND GroupId=@GroupId AND isMainGroup=0
END
GO
