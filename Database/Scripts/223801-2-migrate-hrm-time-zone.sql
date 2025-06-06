use [HRM]

DECLARE @CurrentTimeZone NVARCHAR(50) = 'SE Asia Standard Time';  /* SE Asia Standard Time (UTC+7) / Taipei Standard Time (UTC+8)  */
DECLARE @TargetTimeZone NVARCHAR(50) = 'SA Western Standard Time'; /* SA Western Standard Time (UTC-4) */

/* JobTitle */
UPDATE dbo.[JobTitle]
SET CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* Log */
UPDATE dbo.[Log]
SET LogDate = LogDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* ManagementGrade */
UPDATE dbo.[ManagementGrade]
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* Position */
UPDATE dbo.[Position]
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* ScheduledMessage */
UPDATE dbo.[ScheduledMessage]
SET SendDate = SendDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* TOEIC */
UPDATE dbo.[TOEIC]
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* TrainingAndDevelopment */
UPDATE dbo.[TrainingAndDevelopment]
SET ModifyDate = ModifyDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone