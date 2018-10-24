

CREATE FUNCTION [dbo].[fnc_WorkingTimeRequest_GetRequestByDate_IsOutOfRange](@fromDate1 datetime, @toDate1 datetime, @fromDate2 datetime, @toDate2 datetime)
RETURNS bit
AS
BEGIN
	IF @toDate1 IS NULL
		SET @toDate1 = @fromDate1
	
	DECLARE @ret bit;
	SET @ret  = 0;
	if ( (@fromDate1 > @toDate2) or (@toDate1 < @fromDate2) )
		set @ret = 1;

	RETURN @ret;
END;
