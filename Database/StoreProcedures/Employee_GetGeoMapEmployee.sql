CREATE OR ALTER PROCEDURE [dbo].[Employee_GetGeoMapEmployee] @query NVARCHAR(256)
AS
/*
Created: 20230519@Quinn.Vo
Task: Get geo info of employee.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @q NVARCHAR(256)
    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT e.[Id]
        ,e.[Code]
        ,[FullName]
        ,[NickName]
		,e.GeoMapLongtitude
		,e.GeoMapLatitude
        FROM Employee AS e WITH (NOLOCK)
        WHERE 
			e.Enabled = 1
			AND e.isDeleted = 0
            AND (
                @query IS NULL
                OR e.NickName LIKE @q
                OR e.FullName LIKE @q
                OR e.DomainUsername LIKE @q
                OR e.Code LIKE @q
                )
END

GO
