INSERT INTO [bodb_HRM].[dbo].[Employee]
(
    EmpId,
    Code,
    FullName,
    NickName,
    DOB,
    JobTitleId,
    Phone,
    Gender,
    Skype,
    Yahoo,
    Telegram,
    Marriage,
    PermanentAddress,
    CurrentAddress,
    IdCardNumber,
    PassportNo,
    PassportExp,
    GeoMapLatitude,
    GeoMapLongtitude,
    DistanceToCompany,
    EmployeeCard,
    DateOfIDCard,      -- mapping IssuedDate
    PlaceOfIDCard,     -- mapping IssuedPlace
    PlaceOfBirth,
    ContactName,
    UserRelationship,
    ContactPhone,
    ContactAddress,
    EducationBackground,
    CreatedDate,
    CreatedBy,
    ModifiedDate,
    ModifiedBy,
    isDeleted,
    Enabled,
    JoinedDate,        -- mapping GroupDate
    RefId,
    RefCompanyId,
    OldEmpId
)
SELECT
    Id,
    Code,
    FullName,
    NickName,
    DOB,
    JobTitleId,
    Phone,
    Gender,
    Skype,
    Yahoo,
    Telegram,
    Marriage,
    PermanentAddress,
    CurrentAddress,
    IdCardNumber,
    PassportNo,
    PassportExp,
    GeoMapLatitude,
    GeoMapLongtitude,
    DistanceToCompany,
    EmployeeCard,
    IssuedDate,
    IssuedPlace,
    PlaceOfBirth,
    ContactName,
    UserRelationship,
    ContactPhone,
    ContactAddress,
    EducationBackground,
    CreatedDate,
    CreatedBy,
    ModifiedDate,
    ModifiedBy,
    IsDeleted,
    1,
    GroupDate,
    RefId,
    RefCompanyId,
    OldId              
FROM [HRM].[dbo].[Employee] WITH (NOLOCK)
WHERE UserId IS NOT NULL
GO

UPDATE [NexVisionHRM].[dbo].[SystemSetting] 
SET Value = 34 
WHERE Id = 4
GO

UPDATE [NexVisionHRM].[dbo].[EmployeeExt]
SET Password = N'$2a$11$/xt4J0boH6u4CZJoUbz3b.jWxp1KQxcQXTZoFhKlSycLnkyI6QcCq'
WHERE DomainUsername LIKE '%dylan.le%'
GO