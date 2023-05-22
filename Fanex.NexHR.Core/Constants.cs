namespace Fanex.NexHR.Core
{
    public static class Constants
    {
        public const string MsgLeave = "<span class='req leave'>{1}</span> {0} days";
        public const string MsgLeaveHalfDay = "<span class='req leave'>{0} ({1}-{2})</span> 0.5 day";
        public const string MsgLeaveOneDay = "<span class='req leave'>{0}</span> 1 day";
        public const string MsgCheckInOut = "<span class='req checkinout'>{0} {1}</span> {2}";
        public const string MsgLateIn = "<span class='req inout'>+{0}' {1}</span> {2}";
        public const string MsgEarlyOut = "<span class='req inout'>-{0}' {1}</span> {2}";
        public const string MsgChangeShift = "<span class='req changeshift'>{0} <span class='arr'>&rarr;</span> {1}</span> {2}";

        public const string MailContentLeave = "{0} to {1} ({2} days)";
        public const string MailContentLeaveOneDay = "{0}";
        public const string MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";
        public const string ContentEmployeeWithDay = "- <strong>{0}</strong> ({1}): {2}";
        public const string ContentEmployeeOvertime = "<li><strong>{0}</strong> ({1}) - {2}: {3} hour(s).";

        public const string EmailTemplatePathForRequester = "EmailTemplates/process.request.txt";
        public const string EmailTemplatePathForLeave = "EmailTemplates/Leave.Request.txt";
        public const string EmailTemplatePathForUnpaidLeave = "EmailTemplates/UnpaidLeave.Request.txt";
        public const string EmailTemplatePathForCheckInOut = "EmailTemplates/CheckInOut.Request.txt";
        public const string EmailTemplatePathForMemoNotification = "EmailTemplates/MemoNotification.txt";
        public const string EmailTemplatePathForLeaveApproved = "EmailTemplates/LeaveApprovalNotification.txt";
        public const string EmailTemplatePathForLeaveDisApproved = "EmailTemplates/LeaveDisApprovalNotification.txt";
        public const string EmailTemplatePathForCheckInOutApproved = "EmailTemplates/CheckInOutApprovalNotification.txt";
        public const string EmailTemplatePathForOvertimeRequest = "EmailTemplates/Overtime.Request.txt";
        public const string EmailTemplatePathForOvertimeApprovalNotification = "EmailTemplates/OvertimeApprovalNotification.txt";
        public const string EmailTemplatePathForOvertimeNotification = "EmailTemplates/OvertimeNotification.txt";
        public const string EmailTemplatePathForOvertimeConversion = "EmailTemplates/Overtime.Conversion.txt";
        public const string EmailTemplatePathForTOEICScoreUpdatedNotification = "EmailTemplates/TOEICScoreUpdatedNotification.txt";
        public const string EmailTemplatePathForSuccessUpdateEmployee = "EmailTemplates/SuccessUpdateEmployee.txt";
        public const string EmailTemplatePathForReminderBirthday = "EmailTemplates/ReminderBirthday.txt";
        public const string EmailTemplatePathForReminderFirstWorkingday = "EmailTemplates/ReminderFirstWorkingday.txt";

        public const int CurrentPage = 1;
        public const int ItemsPerPage = 100;

        public const int AnnualLeaveType = 2;
        public const int UnplannedAnnualLeaveType = 30;
        public const int BusinessTripLeaveType = 9;
        public const int UnpaidLeaveType = 26;

        public const string NewLine = "\n\n";
        public const string BrTag = "<br />";

        public const int valid = 2;
        public const int invalid = 3;

        public const int RepeatMinute = 1;
        public const int RepeatHour = 2;
        public const int RepeatDay = 3;
        public const int RepeatWeek = 4;
        public const int RepeatMonth = 5;
        public const int RepeatYear = 6;
        public const int NumberOfDayInWeek = 7;

        public const int TrainingNameLength = 200;
        public const int TrainingOrganizationLength = 200;
        public const int TrainingDescriptionLength = 500;
        public const int TrainingConductedByLength = 100;

    }
}