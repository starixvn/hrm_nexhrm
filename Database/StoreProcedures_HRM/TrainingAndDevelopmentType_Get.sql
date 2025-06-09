USE [HRM]
GO
/****** Object:  StoredProcedure [dbo].[TrainingAndDevelopmentType_Get]    Script Date: 6/9/2025 2:58:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopmentType_Get]
AS
/*
Created: 20230417@Brian.Nguyen
Task: Get training and development type
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [Id]
      ,[Name]
	FROM [dbo].[TrainingAndDevelopmentType] WITH (NOLOCK)

END


GO
