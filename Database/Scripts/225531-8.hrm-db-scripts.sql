

USE [HRM]
GO

sp_rename 'Employee', 'Employee_bk';
GO

CREATE OR ALTER VIEW [dbo].[Employee]

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
  INNER JOIN bodb_HRM.dbo.CompanyEmployee ce WITH(NOLOCK) ON e.EmpId = ce.EmpId AND ce.CompanyId IN (1,2)

GO







