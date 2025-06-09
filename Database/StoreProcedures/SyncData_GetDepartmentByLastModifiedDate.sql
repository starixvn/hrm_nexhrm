USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[SyncData_GetDepartmentByLastModifiedDate]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[SyncData_GetDepartmentByLastModifiedDate] @LastModifiedDate DATETIME
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
		,d.ModifiedDate
	FROM dbo.Department d WITH (NOLOCK)
	WHERE d.ModifiedDate IS NULL OR d.ModifiedDate > @LastModifiedDate
END

GO
