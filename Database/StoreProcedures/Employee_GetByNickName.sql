USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Employee_GetByNickName]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetByNickName] @NickName VARCHAR(50)
AS
/*
Created: 20180829@Quinn.Vo
Task: Get employee info by nick name.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @EmployeeId INT

    SELECT @EmployeeId = Id
    FROM dbo.Employee WITH (NOLOCK)
    WHERE NickName = @NickName

    EXECUTE dbo.Employee_GetById @EmployeeId = @EmployeeId
END

GO
