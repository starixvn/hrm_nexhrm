CREATE PROCEDURE EmployeeGroup_DeleteMember
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