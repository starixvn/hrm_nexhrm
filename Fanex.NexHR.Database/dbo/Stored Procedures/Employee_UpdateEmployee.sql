

CREATE PROCEDURE [dbo].[Employee_UpdateEmployee] @employeeId INT
    ,@code VARCHAR(20) = NULL
    ,@domainUsername VARCHAR(50) = NULL
    ,@fullName NVARCHAR(50) = NULL
    ,@nickName NVARCHAR(20) = NULL
    ,@EmployeeCard VARCHAR(10) = ''
    ,@dob DATETIME = NULL
    ,@email VARCHAR(50) = NULL
    ,@jobTitleId INT = NULL
    ,@enabled BIT = NULL
    ,@phone VARCHAR(50) = NULL
    ,@gender TINYINT = NULL
    ,@mainGroupID INT = NULL
    ,@subGroups VARCHAR(50) = NULL
    ,@HandleRequestGroupIDs VARCHAR(50) = NULL
    ,@Roles VARCHAR(20) = NULL
    ,@joinedDate DATETIME = NULL
    ,@confirmedDate DATETIME = NULL
    ,@resignedDate DATETIME = NULL
    ,@skype VARCHAR(100) = NULL
    ,@yahoo VARCHAR(100) = NULL
    ,@mariage BIT = NULL
    ,@address NVARCHAR(250) = NULL
    ,@currAddress NVARCHAR(250) = NULL
    ,@idCardNo NVARCHAR(20) = NULL
    ,@issuedDate DATETIME = NULL
    ,@issuedPlace NVARCHAR(250) = NULL
    ,@passportNo NVARCHAR(35) = NULL
    ,@passportExp DATETIME = NULL
    ,@geoMapLatitude VARCHAR(50) = ''
    ,@geoMapLongtitude VARCHAR(50) = ''
    ,@distanceToCompany VARCHAR(10) = ''
    ,@modifiedBy INT = NULL
	,@placeOfBirth NVARCHAR(20) = NULL
	,@contactName NVARCHAR(50) = NULL
	,@userRelationship NVARCHAR(10) = NULL
	,@contactPhone VARCHAR(50) = NULL
	,@contactAddress NVARCHAR(250) = NULL
	,@groupDate DATE = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: update employee.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Employee
        SET Code = @code
            ,DomainUsername = @domainUsername
            ,FullName = @fullName
            ,NickName = @nickName
            ,EmployeeCard = @EmployeeCard
            ,DOB = @dob
            ,Email = @email
            ,JobTitleId = @jobTitleId
            ,Enabled = @enabled
            ,isDeleted = CASE 
                WHEN @enabled = 1 THEN 0
                ELSE isDeleted
                END
            ,Phone = @phone
            ,Gender = @gender
            ,GroupId = @mainGroupID
            ,[Roles] = @roles
            ,JoinedDate = @joinedDate
			,GroupDate = @groupDate
            ,ConfirmedDate = @confirmedDate
            ,ResignedDate = @resignedDate
            ,Skype = @skype
            ,Yahoo = @yahoo
            ,Mariage = @mariage
            ,PermanentAddress = @address
            ,CurrentAddress = @currAddress
            ,IdCardNumber = @idCardNo
            ,IssuedDate = @issuedDate
            ,IssuedPlace = @issuedPlace
            ,PassportNo = @passportNo
            ,PassportExp = @passportExp
            ,GeoMapLatitude = @geoMapLatitude
            ,GeoMapLongtitude = @geoMapLongtitude
            ,DistanceToCompany = @distanceToCompany
			,PlaceOfBirth = @placeOfBirth
			,ContactName = @contactName
			,UserRelationship = @userRelationship
			,ContactPhone = @contactPhone
			,ContactAddress = @contactAddress
            ,ModifiedDate = GETDATE()
            ,ModifiedBy = @modifiedBy
        WHERE Id = @employeeId

        DELETE dbo.EmployeeGroup_Employee
        WHERE empid = @employeeId

        INSERT INTO dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @mainGroupID
            ,@employeeId
            ,1
            )

        INSERT INTO dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        SELECT val
            ,@employeeId
            ,0
        FROM dbo.fnc_strSplit(@subGroups, ',')

		INSERT INTO dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
			,HandleRequest
            )
        SELECT val
            ,@employeeId
            ,0
			,1
        FROM dbo.fnc_strSplit(@HandleRequestGroupIDs, ',')

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Employee unsuccessfully.'
    END CATCH;
END


