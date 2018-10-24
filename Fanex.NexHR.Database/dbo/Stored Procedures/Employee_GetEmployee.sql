

--Employee_GetEmployee null, 'spvn\dan.nguyen'
CREATE PROCEDURE [dbo].[Employee_GetEmployee]
	-- Add the parameters for the stored procedure here
	@id int = null,
	@domainUsername varchar(50) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select -- e.*, 
	e.Id,
	e.Code,
	e.DomainUsername,
	e.FullName,
	e.NickName,
	e.EmployeeCard,
	e.DOB as DateOfBirth,
	e.Email,
	e.JobTitleId,
	e.Enabled,
	e.Phone,
	e.Gender,
	e.GroupId,
	e.Roles,
	e.ManagerId,
	e.JoinedDate,
	e.ConfirmedDate,
	e.ResignedDate,
	e.Skype,
	e.Yahoo,
	e.Mariage,
	e.LastLogin,
	e.AnnualLeaveBalance,
	e.CreatedDate,
	e.CreatedBy,
	e.ModifiedDate,
	e.ModifiedBy,
	e.isDeleted,
	e.PermanentAddress as Address,
	e.CurrentAddress,
	e.IdCardNumber as CardNo,
	e.IssuedDate,
	e.IssuedPlace,
	e.PassportNo,
	e.PassportExp,
	e.GeoMapLatitude,
	e.GeoMapLongtitude,
	e.DistanceToCompany,
	isnull(g.ID, -1) as [TeamID], isnull(g.Name,'') as [Team]
	from Employee e WITH (NOLOCK) LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
	where e.Id=@id or DomainUsername=@domainUsername
END
