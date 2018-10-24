
/****** Object:  Stored Procedure dbo.Memo_GetAll    Script Date: 3/13/2016 3:21:42 PM ******/

CREATE PROCEDURE [dbo].[Memo_GetAll] @content NVARCHAR = NULL
    ,@year INT
    ,@category INT
    ,@status INT = - 1
AS
BEGIN
    SELECT m.*
        ,lk.[Name] AS CategoryName
    FROM MEMO m WITH (NOLOCK)
        INNER JOIN LookupData lk WITH (NOLOCK) ON lk.id = m.Category
    WHERE  (
            @year = - 1
            OR year(m.SentDate) = @year
            )
        AND (
            @category = - 1
            OR m.Category = @category
            )
        AND Enabled = 1
        AND (
            @status = - 1
            OR m.STATUS = @status
            )
    ORDER BY m.SentDate DESC
END

