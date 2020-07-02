// --------------------------------------------------------------------------
// <copyright file="Constants.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.3">Created</change>
// </history>

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

        public const string EmailTemplatePathForRequester = "~/app_data/emails/process.request.cshtml";
        public const string EmailTemplatePathForLeave = "~/app_data/emails/Leave.Request.cshtml";
        public const string EmailTemplatePathForUnpaidLeave = "~/app_data/emails/UnpaidLeave.Request.cshtml";
        public const string EmailTemplatePathForCheckInOut = "~/app_data/emails/CheckInOut.Request.cshtml";
        public const string EmailTemplatePathForMemoNotification = "~/app_data/emails/MemoNotification.cshtml";
        public const string EmailTemplatePathForLeaveApproved = "~/app_data/emails/LeaveApprovalNotification.cshtml";
        public const string EmailTemplatePathForCheckInOutApproved = "~/app_data/emails/CheckInOutApprovalNotification.cshtml";
        public const string EmailTemplatePathForOvertimeNotification = "~/app_data/emails/OvertimeNotification.cshtml";
        public const string EmailTemplatePathForTOEICScoreUpdatedNotification = "~/app_data/emails/TOEICScoreUpdatedNotification.cshtml";
        public const string EmailTemplatePathForSuccessUpdateEmployee = "app_data\\emails\\SuccessUpdateEmployee.cshtml";
        public const string EmailTemplatePathForReminderBirthday = "app_data\\emails\\ReminderBirthday.cshtml";
        public const string EmailTemplatePathForReminderFirstWorkingday = "app_data\\emails\\ReminderFirstWorkingday.cshtml";

        public const int CurrentPage = 1;
        public const int ItemsPerPage = 100;

        public const int AnnualLeaveType = 2;
        public const int UnplannedAnnualLeaveType = 30;
        public const int UnpaidLeaveType = 26;

        public const string NewLine = "\n\n";
        public const string BrTag = "<br />";
    }
}