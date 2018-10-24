
/****** Object:  Stored Procedure dbo.EmployeeGroup_GetGroupsByManagerID    Script Date: 3/13/2016 3:21:40 PM ******/
-- [EmployeeGroup_GetGroupsByManagerID] 150

CREATE PROCEDURE [dbo].[EmployeeGroup_GetGroupsByManagerID] 
    @managerID int
AS
/*
Created: 2012/05/18@Dan
Task: Get group of leader or assistant leader
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT * 
    FROM EmployeeGroup
    WHERE (LeaderId = @managerID OR AssistantLeaderId = @managerID)
    AND ISNULL(Deleted,0)=0
    ORDER BY [Name]
END

