use [NexHRM]

DECLARE @CurrentTimeZone NVARCHAR(50) = 'SE Asia Standard Time';  /* SE Asia Standard Time (UTC+7) / Taipei Standard Time (UTC+8)  */
DECLARE @TargetTimeZone NVARCHAR(50) = 'SA Western Standard Time'; /* SA Western Standard Time (UTC-4) */

/* Leave Conversion */
UPDATE dbo.[LeaveConversion]
SET ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* Leave Trans */
UPDATE dbo.[LeaveTrans]
SET FromDate = FromDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ToDate = ToDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* Log */
UPDATE dbo.[Log]
SET LogDate = LogDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* LogReachedWorkingTime */
UPDATE dbo.[LogReachedWorkingTime]
SET InsertedDate = InsertedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* Overtime */
UPDATE dbo.[Overtime]
SET CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* OvertimeConversion */
UPDATE dbo.[OvertimeConversion]
SET	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* OvertimeDetail */
UPDATE dbo.[OvertimeTrans]
SET	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* OvertimeYearly */
UPDATE dbo.[OvertimeYearly]
SET	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* RedmineSpendTimeLog */
UPDATE dbo.[RedmineSpendTimeLog]
SET	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* Shift */
UPDATE dbo.[Shift]
SET	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* TimeRecord */
UPDATE dbo.[TimeRecord]
SET	RecordTime = RecordTime AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	InsertDate = InsertDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* TimeRecordOld */
UPDATE dbo.[TimeRecordOld]
SET	RecordTime = RecordTime AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	InsertDate = InsertDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* tmpEmployeeAttendance */
UPDATE dbo.[tmpEmployeeAttendance]
SET	InsertedDate = InsertedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* WorkFromHomeRequest */
UPDATE dbo.[WorkFromHomeRequest]
SET	ProcessedDate = ProcessedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* WorkingTimeRequest */
UPDATE dbo.[WorkingTimeRequest]
SET	ProcessedDate = ProcessedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	FromDate = FromDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ToDate = ToDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone

/* [WorkingTimeRequest_history] */
UPDATE dbo.[WorkingTimeRequest_history]
SET	ProcessedDate = ProcessedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	FromDate = FromDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ToDate = ToDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	CreatedDate = CreatedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone,
	ModifiedDate = ModifiedDate AT TIME ZONE @CurrentTimeZone AT TIME ZONE @TargetTimeZone