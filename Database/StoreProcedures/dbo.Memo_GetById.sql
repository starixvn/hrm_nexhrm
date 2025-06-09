USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[Memo_GetById]    Script Date: 6/9/2025 11:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Memo_GetById]
    @Id INT
AS
BEGIN
    SELECT m.[Id]
      ,[RefNo]
      ,[Contents]
      ,[Subject]
      ,[SentTo]
      ,[SentFrom]
      ,[ForInfo]
      ,[TotalPages]
      ,[SentDate]
      ,[Category]
      ,[Type] AS MemoType
      ,[Enabled]
      ,[Status]
	  , lk.[Name] AS CategoryName 
    FROM dbo.MEMO m WITH(nolock) 
    INNER JOIN HRM.dbo.LookupData lk WITH(nolock) ON lk.id = m.Category 
    WHERE m.Id = @Id
END

GO
