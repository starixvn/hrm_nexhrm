SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SyncData_GetDepartmentByLastModifiedDate] @LastModifiedDate DATETIME
AS
/*
Created: 20201126@Oliver.Dang
Task: Get Employee to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;

	SELECT d.DepartmentId
		,d.Code
		,d.Name
		,d.ParentId
		,d.ManagerId
		,d.DepartmentLevel
		,(
			CASE 
				WHEN ISNULL(d.IsDisabled, 0) = 1
					THEN 0
				ELSE 1
				END
			) AS IsActive
		,d.ModifiedBy
	FROM dbo.Department d WITH (NOLOCK)
	WHERE d.ModifiedDate > @LastModifiedDate
END
