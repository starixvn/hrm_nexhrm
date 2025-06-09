USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[EmployeeLogTimeMissed_UpdateMultiple]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[EmployeeLogTimeMissed_UpdateMultiple]
    @EmpIDs NVARCHAR(MAX) 
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20250228@Henry.dinh
Task: Insert/Update missed logging by date
DB: NexHRM
Original: EmployeeLogTimeMissed_UpsertMultipleByDate
Revisions: 
    - 20250307 - Changed to use Date instead of Month/Year
Param's Explanation:
- @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
- @ErrMsg: Error description.
*/
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @Yesterday DATE = 
            CASE 
                WHEN DATEPART(WEEKDAY, GETDATE()) = 2 THEN
                    DATEADD(DAY, -3, GETDATE())
                ELSE
                    DATEADD(DAY, -1, GETDATE())
            END;
            
        DECLARE @EmpTable TABLE (EmpID INT);
        
        INSERT INTO @EmpTable (EmpID) SELECT value FROM STRING_SPLIT(@EmpIDs, ',');
        
        INSERT INTO [dbo].[EmployeeLogTimeMissed] (EmpID, [Date])
        SELECT source.EmpID, @Yesterday
        FROM @EmpTable AS source
        LEFT JOIN [dbo].[EmployeeLogTimeMissed] AS target
            ON target.EmpID = source.EmpID AND target.[Date] = @Yesterday
        WHERE target.EmpID IS NULL;
            
        SET @ErrCode = 0;
        SET @ErrMsg = '';
    END TRY
    BEGIN CATCH
        SET @ErrCode = ERROR_NUMBER();
        SET @ErrMsg = ERROR_MESSAGE();
    END CATCH
    
    SET NOCOUNT OFF;
END
GO
