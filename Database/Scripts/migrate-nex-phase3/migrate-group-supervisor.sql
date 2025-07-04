USE [NexHRM]
GO

DECLARE @ActionType INT = 3 -- Approver
DECLARE @RequestType INT = 6 -- Leave

INSERT INTO [bodb_HRM].[dbo].[GroupSupervisor] (
    [OwnerId],
    [GroupId],
    [ActionType],
    [RequestType],
    [ModifiedBy],
    [ModifiedDate],
    [IsDeleted],
    [DisplayOrder]
)
SELECT 
    g.ApproverId,
    g.GroupId,
    @ActionType,
    @RequestType,
    g.ModifiedBy,
    g.ModifiedDate,
    g.IsDeleted,
    1
FROM [dbo].[GroupSupervisor] g

EXEC sp_rename 'GroupSupervisor', 'GroupSupervisor_bk';
GO
