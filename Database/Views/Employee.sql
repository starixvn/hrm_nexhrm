ALTER VIEW [dbo].[Employee]
AS
SELECT e.Id, e.Code, ext.DomainUsername, e.FullName, (CASE WHEN ext.NickName2 IS NULL THEN e.NickName ELSE ext.NickName2 END) AS NickName
    ,e.DOB, ext.Email, e.JobTitleId, ext.[Enabled], e.Phone, e.Gender, ext.GroupId, ext.Roles
    ,ext.JoinedDate, ext.ConfirmedDate, ext.ResignedDate, e.Skype, e.Yahoo, e.Marriage
    ,e.CreatedDate, e.CreatedBy, e.ModifiedDate, e.ModifiedBy, ext.IsDeleted, e.PermanentAddress, e.CurrentAddress, e.IdCardNumber, e.PassportNo
    ,e.PassportExp, e.GeoMapLatitude, e.GeoMapLongtitude, e.DistanceToCompany, e.EmployeeCard, e.IssuedDate, e.IssuedPlace, ext.IsSpecialUser, e.GroupDate
    ,e.PlaceOfBirth, e.ContactName, e.UserRelationship, e.ContactPhone
    ,e.ContactAddress, e.EducationBackground, e.Telegram, e.SocialInsuranceNo, ec.CompanyId, ext.SalaryGroupId
FROM HRM.dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN HRM.dbo.EmployeeCompany AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
    INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON ec.CompanyId = st.[Value] AND st.Id = 4
    INNER JOIN dbo.EmployeeExt AS ext WITH (NOLOCK) ON e.Id = ext.Id
GO