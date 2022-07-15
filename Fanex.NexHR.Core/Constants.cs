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

        public const string EmailTemplatePathForRequester = "~/app_data/emails/process.request.cshtml";
        public const string EmailTemplatePathForLeave = "app_data\\emails\\Leave.Request.cshtml";
        public const string EmailTemplatePathForUnpaidLeave = "app_data\\emails\\UnpaidLeave.Request.cshtml";
        public const string EmailTemplatePathForCheckInOut = "app_data\\emails\\CheckInOut.Request.cshtml";
        public const string EmailTemplatePathForMemoNotification = "~/app_data/emails/MemoNotification.cshtml";
        public const string EmailTemplatePathForLeaveApproved = "~/app_data/emails/LeaveApprovalNotification.cshtml";
        public const string EmailTemplatePathForLeaveDisApproved = "~/app_data/emails/LeaveDisApprovalNotification.cshtml";
        public const string EmailTemplatePathForCheckInOutApproved = "~/app_data/emails/CheckInOutApprovalNotification.cshtml";
        public const string EmailTemplatePathForOvertimeRequest = "~/app_data/emails/Overtime.Request.cshtml";
        public const string EmailTemplatePathForOvertimeApprovalNotification = "~/app_data/emails/OvertimeApprovalNotification.cshtml";
        public const string EmailTemplatePathForOvertimeNotification = "~/app_data/emails/OvertimeNotification.cshtml";
        public const string EmailTemplatePathForOvertimeConversion = "~/app_data/emails/Overtime.Conversion.cshtml";
        public const string EmailTemplatePathForTOEICScoreUpdatedNotification = "~/app_data/emails/TOEICScoreUpdatedNotification.cshtml";
        public const string EmailTemplatePathForSuccessUpdateEmployee = "app_data\\emails\\SuccessUpdateEmployee.cshtml";
        public const string EmailTemplatePathForReminderBirthday = "app_data\\emails\\ReminderBirthday.cshtml";
        public const string EmailTemplatePathForReminderFirstWorkingday = "app_data\\emails\\ReminderFirstWorkingday.cshtml";

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
    }
}