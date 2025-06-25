CREATE OR ALTER FUNCTION fnc_GetClientDateTime()
RETURNS DATETIME
AS
BEGIN
    DECLARE @ClientTimeZone NVARCHAR(50) = 'SE Asia Standard Time'; /* SE Asia Standard Time (UTC+7) */
    DECLARE @CurrentTimeZone NVARCHAR(50) = 'SA Western Standard Time'; /* SA Western Standard Time (UTC-4) */
    
    RETURN GETDATE() AT TIME ZONE @CurrentTimeZone AT TIME ZONE @ClientTimeZone
END
GO