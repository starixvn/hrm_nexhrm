USE [NexHRM]
GO

DECLARE @ActionType INT = 3 -- Approver
DECLARE @RequestType INT = 6 -- Leave

INSERT INTO [bodb_HRM].[dbo].[EmployeeSupervisor] (
    [OwnerId],
    [EmpId],
    [ActionType],
    [RequestType],
    [ModifiedBy],
    [ModifiedDate],
    [IsDeleted],
    [OrderNo],
    [OrderCode]
)
SELECT 
    es.ApproverId,
    es.EmpId,
    @ActionType,
    @RequestType,
    es.ModifiedBy,
    es.ModifiedDate,
    es.IsDeleted,
    1,
    NULL
FROM [dbo].[EmployeeSupervisor] es
