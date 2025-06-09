USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[Lookup_GetLookupData]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[Lookup_GetLookupData]
    @lookupCode varchar(20)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @lookupId int
    SELECT @lookupId = Id FROM dbo.[Lookup] WITH (NOLOCK) WHERE Code = @lookupCode
    
    SELECT * FROM dbo.LookupData WITH (NOLOCK) WHERE LookupId = @lookupId
    ORDER BY [Name]
END


GO
