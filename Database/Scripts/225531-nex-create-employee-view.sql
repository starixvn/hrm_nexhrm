USE [NexHRM]

GO

CREATE OR ALTER VIEW [dbo].[Employee]
AS


SELECT e.EmpId as Id, e.OldEmpId as OldId, e.Code, ec.DomainUsername, e.FullName, (CASE WHEN ec.NickName2 IS NULL THEN e.NickName ELSE ec.NickName2 END) AS NickName
    ,e.DOB, ec.Email, e.JobTitleId, ec.[Enabled], e.Phone, e.Gender, ec.GroupId, ec.Roles
    ,ec.JoinedDate, ec.ConfirmedDate, ec.ResignedDate, e.Skype, e.Yahoo, e.Marriage
    ,e.CreatedDate, e.CreatedBy, e.ModifiedDate, e.ModifiedBy, ec.IsDeleted, e.PermanentAddress, e.CurrentAddress, e.IdCardNumber, e.PassportNo
    ,e.PassportExp, e.GeoMapLatitude, e.GeoMapLongtitude, e.DistanceToCompany, e.EmployeeCard, e.DateOfIDCard as IssuedDate, e.PlaceOfIDCard as IssuedPlace, ec.IsSpecialUser, e.GroupDate
    ,e.PlaceOfBirth, e.ContactName, e.UserRelationship, e.ContactPhone
    ,e.ContactAddress, e.EducationBackground, e.Telegram, e.SocialInsuranceNo, ec.CompanyId, ec.SalaryGroupId
FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ec WITH (NOLOCK) ON e.EmpId = ec.EmpId 
    INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON ec.CompanyId = st.[Value] AND st.Id = 4
GO


CREATE OR ALTER VIEW [dbo].[Employee2]
AS
SELECT e.EmpId as Id, e.Code, ec.DomainUsername, e.FullName, (CASE WHEN ec.NickName2 IS NULL THEN e.NickName ELSE ec.NickName2 END) AS NickName, e.DOB, ec.Email, e.JobTitleId, ec.Enabled, e.Phone, e.Gender, ec.GroupId, ec.JoinedDate, 
                  ec.ConfirmedDate, ec.ResignedDate, e.Skype, e.Yahoo, e.Marriage, e.CreatedDate, e.CreatedBy, e.ModifiedDate, e.ModifiedBy, ec.IsDeleted, e.EmployeeCard, ec.IsSpecialUser, e.GroupDate, e.PlaceOfBirth, e.EducationBackground, 
                  e.Telegram
FROM     bodb_HRM.dbo.Employee AS e WITH (NOLOCK) 
				INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ec WITH (NOLOCK) ON e.EmpId = ec.EmpId 
					INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON ec.CompanyId = st.[Value] AND st.Id = 4
WHERE  (ec.IsSpecialUser = 0)
GO


CREATE OR ALTER VIEW [dbo].[Department]

AS
SELECT d.DepartmentId
      ,d.Code
      ,d.[Name]
      ,0 as ParentId
      ,d.PICEmpId as ManagerId
	  ,NULL as [DepartmentLevel]
      ,NULL as [DisplayOrder]
      ,(
			CASE 
				WHEN ISNULL(d.IsActive, 1) = 0
					THEN 1
				ELSE 0
				END
			) AS  IsDisabled
      ,d.ModifiedBy as CreatedBy
      ,d.ModifiedDate as CreatedDate
      ,d.ModifiedBy
      ,d.ModifiedDate
      ,0 as IsSpecial
  FROM bodb_HRM_bk.dbo.Department d

GO



USE [HRM_bk]
GO


CREATE OR ALTER VIEW [dbo].[Employee_bk1]

AS

SELECT e.[EmpId] as Id 
      ,e.[Code]
      ,e.[FullName]
      ,e.[NickName]
      ,e.[DOB]
      ,e.[JobTitleId]
      ,e.[Phone]
      ,e.[Gender]
      ,e.[Skype]
      ,e.[Yahoo]
      ,e.[Telegram]
      ,e.[Marriage]
      ,e.[PermanentAddress]
      ,e.[CurrentAddress]
      ,e.[IdCardNumber]
      ,e.[PassportNo]
      ,e.[PassportExp]
      ,e.[GeoMapLatitude]
      ,e.[GeoMapLongtitude]
      ,e.[DistanceToCompany]
      ,e.[EmployeeCard]
      ,e.[DateOfIDCard] as IssuedDate
      ,e.[PlaceOfIDCard] as IssuedPlace
      ,e.[PlaceOfBirth]
      ,e.[ContactName]
      ,e.[UserRelationship]
      ,e.[ContactPhone]
      ,e.[ContactAddress]
      ,e.[EducationBackground]
      ,e.[SocialInsuranceNo]
      ,e.[JoinedDate] as GroupDate
      ,e.[CreatedDate]
      ,e.[CreatedBy]
      ,e.[ModifiedDate]
      ,e.[ModifiedBy]
      ,e.[RefId]
      ,e.[RefCompanyId]
      ,e.[IsDeleted]
  FROM bodb_HRM.dbo.Employee e
  INNER JOIN bodb_HRM.dbo.CompanyEmployee ce WITH(NOLOCK) ON e.EmpId = ce.EmpId AND ce.CompanyId IN (3,4)

GO







