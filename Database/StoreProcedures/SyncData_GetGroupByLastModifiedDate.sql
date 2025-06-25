CREATE OR ALTER PROCEDURE [dbo].[SyncData_GetGroupByLastModifiedDate] @LastModifiedDate DATETIME
AS
/*
Created: 20201126@Oliver.Dang
Task: Get Employee to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;

	SELECT g.Id as GroupId
		  ,g.Code
		  ,g.Name
		  ,g.DepartmentId
		  ,g.[Rank]
		  ,ISNULL(g.Deleted,0) as IsDeleted
		  ,g.ModifiedDate
	FROM dbo.EmployeeGroup g WITH (NOLOCK)
	WHERE g.ModifiedDate > @LastModifiedDate
END

GO
