
/****** Object:  Stored Procedure dbo.Memo_GetById    Script Date: 3/13/2016 3:21:42 PM ******/




CREATE PROCEDURE [dbo].[Memo_GetById]
    @Id INT
AS
BEGIN
    SELECT m.*, lk.[Name] AS CategoryName 
    FROM MEMO m WITH(nolock) 
    INNER JOIN LookupData lk WITH(nolock) ON lk.id = m.Category 
    WHERE m.Id = @Id
END

