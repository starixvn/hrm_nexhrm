CREATE OR ALTER PROCEDURE [dbo].[Employee_GetRedmineUserByDomainName]
	-- Add the parameters for the stored procedure here
	@LoginName NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT rU.Id as RedmineId
		,rU.Mail as Email
		,rU.FirstName as FirstName
		,rU.LastName as LastName
	FROM [RedmineNexcel].[dbo].[Users] as rU WITH (NOLOCK)
	WHERE @LoginName = rU.[Login]
END
GO
