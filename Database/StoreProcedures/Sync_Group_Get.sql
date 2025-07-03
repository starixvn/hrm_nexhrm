CREATE OR ALTER PROCEDURE [dbo].[Sync_Group_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get group by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT ISNULL(Id, OldId) AS GroupId, 
		[Name], 
		Code, 
		ParentId, 
		CASE 
			WHEN DepartmentId IS NULL OR DepartmentId = 0 
				THEN 999 
			WHEN DepartmentId IS NOT NULL 
				THEN DepartmentId 
		END AS DepartmentId, 
		ISNULL(Deleted, 0) AS IsDeleted, 
		[Rank],
		ParentId,
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM dbo.EmployeeGroup WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END