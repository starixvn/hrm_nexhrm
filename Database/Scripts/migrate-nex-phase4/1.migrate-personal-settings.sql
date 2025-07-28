USE [NexHRM]
GO

DECLARE @ActionType INT = 3 -- Approver
DECLARE @RequestType INT = 6 -- Leave

MERGE [bodb_HRM].[dbo].[EmployeeSupervisor] AS target
USING (
    SELECT 
        es.ApproverId,
        es.EmpId,
        @ActionType AS ActionType,
        @RequestType AS RequestType,
        es.ModifiedBy,
        es.ModifiedDate,
        es.IsDeleted,
        1 AS OrderNo,
        NULL AS OrderCode
    FROM [dbo].[EmployeeSupervisor] es
) AS source
ON (
    target.OwnerId = source.ApproverId 
    AND target.EmpId = source.EmpId 
    AND target.ActionType = source.ActionType 
    AND target.RequestType = source.RequestType
)
WHEN MATCHED THEN
    UPDATE SET
        target.IsDeleted = source.IsDeleted,
        target.ModifiedBy = source.ModifiedBy,
        target.ModifiedDate = source.ModifiedDate,
        target.OrderNo = source.OrderNo,
        target.OrderCode = source.OrderCode
WHEN NOT MATCHED THEN
    INSERT (
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
    VALUES (
        source.ApproverId,
        source.EmpId,
        source.ActionType,
        source.RequestType,
        source.ModifiedBy,
        source.ModifiedDate,
        source.IsDeleted,
        source.OrderNo,
        source.OrderCode
    );
