
/****** Object:  Stored Procedure dbo.EmployeeShift_Get    Script Date: 3/13/2016 3:21:41 PM ******/
CREATE PROCEDURE [dbo].[EmployeeShift_Get]
    @EmployeeId INT
AS

/*
Created: 20160116@Quinn.Vo
Task: Get employee default schedule.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/

BEGIN
    SET NOCOUNT ON;

    SELECT es.EmpID,
       e.NickName,
       e.FullName,
       [MonShift] AS MonShiftId,
        ISNULL(s1.Name, 'OFF') AS MonShiftName,
        [TueShift] AS TueShiftId,
        ISNULL(s2.Name, 'OFF') AS TueShiftName,
        [WedShift] AS WedShiftId,
        ISNULL(s3.Name, 'OFF') AS WedShiftName,
        [ThuShift] AS ThuShiftId,
        ISNULL(s4.Name, 'OFF') AS ThuShiftName,
        [FriShift] AS FriShiftId,
        ISNULL(s5.Name, 'OFF') AS FriShiftName,
        [SatShift] AS SatShiftId,
        ISNULL(s6.Name, 'OFF') AS SatShiftName,
        [SunShift] AS SunShiftId,
        ISNULL(s7.Name, 'OFF') AS SunShiftName
    FROM [dbo].[EmployeeShift] AS es WITH (NOLOCK)
       INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpID = e.Id
        LEFT JOIN dbo.[Shift] AS s1 WITH (NOLOCK) ON es.MonShift = s1.Id
        LEFT JOIN dbo.[Shift] AS s2 WITH (NOLOCK) ON es.TueShift = s2.Id
        LEFT JOIN dbo.[Shift] AS s3 WITH (NOLOCK) ON es.WedShift = s3.Id
        LEFT JOIN dbo.[Shift] AS s4 WITH (NOLOCK) ON es.ThuShift = s4.Id
        LEFT JOIN dbo.[Shift] AS s5 WITH (NOLOCK) ON es.FriShift = s5.Id
        LEFT JOIN dbo.[Shift] AS s6 WITH (NOLOCK) ON es.SatShift = s6.Id
        LEFT JOIN dbo.[Shift] AS s7 WITH (NOLOCK) ON es.SunShift = s7.Id
    WHERE EmpID = @EmployeeId;
END;
