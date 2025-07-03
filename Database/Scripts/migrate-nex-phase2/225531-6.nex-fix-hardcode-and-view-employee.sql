USE [NexHRM]
GO

INSERT INTO [dbo].[SystemSetting]
           (
            [Id]
           ,[Name]
           ,[Value])
     VALUES
           (6, 'HRTeamId', 300000509),
           (7, 'CoreTeamId', 300000502),
           (8, 'CEOTeamId', 300000504),
           (9, 'ITNetWorkTeamId', 300000510),
           (10, 'EXPTeamId', 300000503),
           (11, 'FATeamId', 300000505),
           (12, 'MGTHeadCode', '02197')
GO




EXEC sp_rename 'Employee', 'Employee_bk';
EXEC sp_rename 'EmployeeExt', 'EmployeeExt_bk';
EXEC sp_rename 'EmployeeGroup', 'EmployeeGroup_bk';
EXEC sp_rename 'Department', 'Department_bk';

GO


CREATE OR ALTER VIEW [dbo].[Employee]
AS


SELECT e.EmpId as Id, e.OldEmpId as OldId, e.Code, ec.DomainUsername, e.FullName, (CASE WHEN ec.NickName2 IS NULL THEN e.NickName ELSE ec.NickName2 END) AS NickName
    ,e.DOB, ec.Email, e.JobTitleId, ec.[Enabled], e.Phone, e.Gender, ec.GroupId, ec.Roles
    ,ec.CompanyDate as JoinedDate, ec.ConfirmedDate, ec.ResignedDate, e.Skype, e.Yahoo, e.Marriage
    ,e.CreatedDate, e.CreatedBy, e.ModifiedDate, e.ModifiedBy, ec.IsDeleted, e.PermanentAddress, e.CurrentAddress, e.IdCardNumber, e.PassportNo
    ,e.PassportExp, e.GeoMapLatitude, e.GeoMapLongtitude, e.DistanceToCompany, e.EmployeeCard, e.DateOfIDCard as IssuedDate, e.PlaceOfIDCard as IssuedPlace, ec.IsSpecialUser, e.JoinedDate as GroupDate
    ,e.PlaceOfBirth, e.ContactName, e.UserRelationship, e.ContactPhone
    ,e.ContactAddress, e.EducationBackground, e.Telegram, e.SocialInsuranceNo, ec.CompanyId, ec.SalaryGroupId
FROM bodb_HRM.dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ec WITH (NOLOCK) ON e.EmpId = ec.EmpId 
    INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON ec.CompanyId = st.[Value] AND st.Id = 4
GO


CREATE OR ALTER VIEW [dbo].[Employee2]
AS
SELECT e.EmpId as Id, e.Code, ec.DomainUsername, e.FullName, (CASE WHEN ec.NickName2 IS NULL THEN e.NickName ELSE ec.NickName2 END) AS NickName, e.DOB, ec.Email, e.JobTitleId, ec.Enabled, e.Phone, e.Gender, ec.GroupId, ec.CompanyDate as JoinedDate, 
                  ec.ConfirmedDate, ec.ResignedDate, e.Skype, e.Yahoo, e.Marriage, e.CreatedDate, e.CreatedBy, e.ModifiedDate, e.ModifiedBy, ec.IsDeleted, e.EmployeeCard, ec.IsSpecialUser, e.JoinedDate as GroupDate, e.PlaceOfBirth, e.EducationBackground, 
                  e.Telegram
FROM     bodb_HRM.dbo.Employee AS e WITH (NOLOCK) 
				INNER JOIN bodb_HRM.dbo.CompanyEmployee AS ec WITH (NOLOCK) ON e.EmpId = ec.EmpId 
					INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON ec.CompanyId = st.[Value] AND st.Id = 4
WHERE  (ec.IsSpecialUser = 0)
GO

CREATE OR ALTER VIEW [dbo].[EmployeeGroup]

AS 
SELECT 
		g.GroupId as Id
    ,g.OldGroupId as OldId
    ,g.GroupTeamId
      ,g.Code
      ,g.Name
      ,g.DepartmentId
      ,g.PICEmpId as LeaderId
      ,g.PICEmpId as AssistantLeaderId
      ,g.[Rank]
      ,g.CreatedDate
      ,g.CreatedBy
      ,g.ModifiedDate
      ,g.ModifiedBy
      ,g.IsDeleted as Deleted
      ,0 as ParentId
      ,0 as IsSpecialTeam
  FROM bodb_HRM.dbo.[Group] g
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON g.CompanyId = st.[Value] AND st.Id = 4

GO


CREATE OR ALTER VIEW [dbo].[Department]

AS
SELECT DISTINCT d.DepartmentId
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
  FROM bodb_HRM.dbo.Department d
  INNER JOIN bodb_HRM.dbo.[Group] g WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
  INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON g.CompanyId = st.[Value] AND st.Id = 4
GO
