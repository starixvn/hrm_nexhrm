
CREATE FUNCTION [dbo].[RelatedDepartment] (
	@DepartmentId INT
	)
RETURNS @Departments TABLE (
	DepartmentId INT,
	DepartmentLevel INT
	)
AS
/*
Created: 20140325@Quinn.Vo
Task: Split string by character.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN

	INSERT INTO @Departments (DepartmentId, DepartmentLevel)
	SELECT DepartmentId, DepartmentLevel
	FROM dbo.Department WITH (NOLOCK)
	WHERE ParentId = @DepartmentId

	-- Add new child records until exhausted.
	WHILE @@ROWCOUNT > 0
	INSERT INTO @Departments (DepartmentId, DepartmentLevel)
	SELECT dp.DepartmentId, dp.DepartmentLevel
	FROM dbo.Department AS dp
	INNER JOIN @Departments AS d ON dp.ParentID = d.DepartmentId
	WHERE NOT EXISTS(
		SELECT 1 FROM @Departments AS CurrentRecords 
		WHERE CurrentRecords.DepartmentId= dp.DepartmentId
		)

	RETURN
END
