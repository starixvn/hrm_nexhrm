USE HRM
GO

DECLARE @CurrentTimeZone NVARCHAR(50) = 'SE Asia Standard Time'; /* SE Asia Standard Time (UTC+7) / Taipei Standard Time (UTC+8)  */
DECLARE @TargetTimeZone NVARCHAR(50) = 'SA Western Standard Time'; 

UPDATE Applicant
SET CreatedDate = CreatedDate AT TIME ZONE @TargetTimeZone  AT TIME ZONE @CurrentTimeZone,
	InterviewDate1 = CreatedDate AT TIME ZONE @TargetTimeZone  AT TIME ZONE @CurrentTimeZone,
	InterviewDate2 = InterviewDate2 AT TIME ZONE @TargetTimeZone  AT TIME ZONE @CurrentTimeZone;
	
--UPDATE BioCard

UPDATE BioDevice
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

UPDATE BioFingerprint
SET LastUpdatedAt = LastUpdatedAt AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

UPDATE BioUserCard
SET ISSDDT = ISSDDT AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	LastUpdatedAt = LastUpdatedAt AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

--UPDATE Conversation

--UPDATE ConversationPermission

UPDATE Employee
SET CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

--UPDATE EmployeeCompany

UPDATE EmployeeJobGrade
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

UPDATE EmployeeManagementGrade
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

UPDATE JobGrade
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

/* JobTitle */
UPDATE dbo.[JobTitle]
SET CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

/* Log */
UPDATE dbo.[Log]
SET LogDate = LogDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

/* ManagementGrade */
UPDATE dbo.[ManagementGrade]
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

/* Position */
UPDATE dbo.[Position]
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

/* ScheduledMessage */
UPDATE dbo.[ScheduledMessage]
SET SendDate = SendDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

/* TOEIC */
UPDATE dbo.[TOEIC]
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;

/* TrainingAndDevelopment */
UPDATE dbo.[TrainingAndDevelopment]
SET ModifyDate = ModifyDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone;
