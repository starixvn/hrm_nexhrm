namespace Fanex.NexHR.WebSite
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Optimization;

    public static class BundleConfig
    {
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public static void RegisterBundles()
        {
            var bundles = BundleTable.Bundles;

            BuildBundleForCommon(bundles);

            BuildBundleForDashboard(bundles);

            BuildBundleForLeaveRequest(bundles);

            BuildBundleForCheckInOutRequest(bundles);

            BuildBundleForMyTimeSheetReport(bundles);

            BuildBundleForReportMyTimeRecord(bundles);

            BuildBundleForMyCalendar(bundles);

            BuildBundleForMemberRequests(bundles);

            BuildBundleForRequestsStatistic(bundles);

            BuildBundleForAnnualLeaveBalance(bundles);

            BuildBundleForEditLeaveTransaction(bundles);

            BuildBundleForTotalWorkingHours(bundles);

            BuildBundleForHolidays(bundles);

            BuildBundleForGeolocation(bundles);

            BuildBundleForExtension(bundles);

            BuildBundleForMemo(bundles);

            BuildBundleForUserManagement(bundles);

            BuildBundleForTeamManagement(bundles);

            BuildBundleForToeic(bundles);

            BuildBundleForJobTitle(bundles);

            BuildBundleForAttendanceReport(bundles);

            BuildBundleForEditEmployee(bundles);

            BuildBundleForLateInEarlyOut(bundles);

            BuildBundleForLateInEarlyOutDiscipline(bundles);

            BuildBundleForDefaultSchedule(bundles);

            BuildBundleForMonthSchedule(bundles);

            BuildBundleForReportTimeRecord(bundles);

            BuildBundleForLeaveSummary(bundles);

            BuildBundleForWorkingCanlendar(bundles);

            BuildBundleForStaffInfo(bundles);

            BuildBundleForStaffBirthday(bundles);

            BuildBundleForLeaveDeductionDetail(bundles);

            BuildBundleForLeaveConversionDetail(bundles);

            BuildBundlerForOvertime(bundles);

            BuildBundleForLateInEarlyOutDetail(bundles);

            BuildBundleForMyProfile(bundles);

            BuildBundleForSimplePagination(bundles);

            BuildBundleForLateEarly(bundles);

            BuildBundleForPostnatal(bundles);

            BuildBundleForMyRequest(bundles);

            BuildBundleForFullCalendar(bundles);

            BuildBundleForContextMenu(bundles);

            BuildBundleForMonthPicker(bundles);
        }

        private static void BuildBundleForCommon(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/sitejs").Include(
                         "~/Scripts/Libs/jquery-2.1.4.min.js",
                         "~/Scripts/Libs/jquery-ui-{version}.js",
                         "~/Scripts/Libs/jquery.validate*",
                         "~/Scripts/Libs/jquery-dateFormat.min.js",
                         "~/Scripts/Libs/materialize.js",
                         "~/Scripts/Libs/jquery.tipsy.js",
                         "~/Scripts/Libs/jsrender.js",
                         "~/Scripts/Libs/jquery.cookie.js",
                         "~/Scripts/Libs/chart.js",
                         "~/Scripts/Libs/date.js",
                         "~/Scripts/Libs/jquery.floatThead.min.js",
                         "~/Scripts/Libs/common.js",
                         "~/Scripts/Views/language.js",
                         "~/Scripts/Notification/notification.js"));

            bundles.Add(new StyleBundle("~/Content/sitecss")
                  .Include("~/Content/Materialize/materialize.css", new CssRewriteUrlTransform())
                  .Include("~/Content/Common/components/materialdesignicons.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/core.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/accordion.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/autocomplete.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/button.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/datepicker.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/dialog.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/draggable.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/menu.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/progressbar.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/resizable.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/selectable.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/sortable.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/slider.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/spinner.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/tabs.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/tooltip.css", new CssRewriteUrlTransform())
                  .Include("~/Content/themes/base/theme.css", new CssRewriteUrlTransform())
                  .Include("~/Content/site.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundlerForOvertime(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/manageovertimejs").Include(
                         "~/Scripts/Views/Overtime/manage.js"));

            bundles.Add(new ScriptBundle("~/bundles/overtimesummaryjs").Include(
             "~/Scripts/Views/Employee/_employee.search.in.company.js",
             "~/Scripts/Views/Overtime/overtimeSummary.js"));

            bundles.Add(new ScriptBundle("~/bundles/overtimejs").Include(
                "~/Scripts/Views/Overtime/myOvertime.js",
                "~/Scripts/Views/Overtime/overtime.member.js",
                "~/Scripts/Views/Overtime/overtimeRequest.js",
                "~/Scripts/Views/Overtime/overtime.js"));
        }

        private static void BuildBundleForMyRequest(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/myrequestcss")
                  .Include("~/Content/Views/myrequest.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForTotalWorkingHours(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/totalworkinghourjs").Include(
                "~/Scripts/Views/Employee/_employee.search.in.company.js",
                "~/Scripts/Views/Report/report.totalworkinghours.js"));
        }

        private static void BuildBundleForHolidays(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/holidays").Include(
                "~/Scripts/Views/Holiday/holiday.js"));
            bundles.Add(new ScriptBundle("~/bundles/holidayedit").Include(
                "~/Scripts/Views/Holiday/holiday.edit.js"));
        }

        private static void BuildBundleForGeolocation(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/geolocationjs").Include(
                "~/Scripts/Views/Geolocationmap/geolocationmap.js"));

            bundles.Add(new StyleBundle("~/Content/geolocationcss")
                  .Include("~/Content/Views/geolocation.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForExtension(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/addextensionnojs").Include(
                "~/Scripts/Views/ExtensionNo/addextensionno.js"));

            bundles.Add(new ScriptBundle("~/bundles/extensionnojs").Include(
                "~/Scripts/Views/ExtensionNo/extensionno.js"));
        }

        private static void BuildBundleForMemo(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/memoindexjs").Include(
                "~/Scripts/Views/Memo/memo.js",
                "~/Scripts/Views/Memo/memo.index.js"));

            bundles.Add(new ScriptBundle("~/bundles/memoeditjs").Include(
                "~/Scripts/Views/Memo/memo.js",
                "~/Scripts/tiny_mce/tiny_mce_src.js",
                "~/Scripts/Views/Memo/memo.edit.js"));

            bundles.Add(new StyleBundle("~/Content/annualleavebalancecss")
                  .Include("~/Content/Views/annualleavebalance.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForToeic(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/toeicjs").Include(
                "~/Scripts/Views/Employee/employee.toeic.js"));
        }

        private static void BuildBundleForLeaveRequest(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/leavejs").Include(
                "~/Scripts/Views/Request/leave.request.js",
                "~/Scripts/Views/Request/process.leave-checkinout.byemployee.js",
                "~/Scripts/Views/Request/search.request.js"));
        }

        private static void BuildBundleForCheckInOutRequest(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/empcheckinoutjs").Include(
                "~/Scripts/Views/Request/checkinout.request.js",
                "~/Scripts/Views/Request/process.leave-checkinout.byemployee.js",
                "~/Scripts/Views/Request/search.request.js"));
        }

        private static void BuildBundleForMyTimeSheetReport(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/mytimesheetjs").Include(
                "~/Scripts/Views/Report/report.timesheet.js"));

            bundles.Add(new StyleBundle("~/Content/mytimesheetcss")
                  .Include("~/Content/Views/report.timesheet.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForMyCalendar(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/mycalendarjs").Include(
                "~/Scripts/Views/Calendar/mycalendar.js"));

            bundles.Add(new StyleBundle("~/Content/mycalendarcss")
                  .Include("~/Content/Views/mycalendar.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForMemberRequests(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/managerequestsjs").Include(
                "~/Scripts/Views/Request/mgr.requests.js",
                "~/Scripts/Views/Employee/_employee.search.in.team.js"));
        }

        private static void BuildBundleForRequestsStatistic(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/reportmonitoringjs").Include(
                "~/Scripts/Views/Report/report.monitoring.js"));
        }

        private static void BuildBundleForAnnualLeaveBalance(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/annualleavebalancejs").Include(
                "~/Scripts/Views/AnnualLeave/index.js",
                "~/Scripts/Views/Employee/_employee.search.in.company.js"));

            bundles.Add(new ScriptBundle("~/bundles/employeeannualleavebalancejs").Include(
                "~/Scripts/Views/AnnualLeave/employee.js"));
        }

        private static void BuildBundleForEditLeaveTransaction(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/editleavetransactionjs").Include(
                "~/Scripts/Views/AnnualLeave/annualleave.edit.js"));
        }

        private static void BuildBundleForJobTitle(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jobtitlejs").Include(
                "~/Scripts/Views/Jobtitle/jobtitle.js"));

            bundles.Add(new StyleBundle("~/Content/jobtitlecss")
                  .Include("~/Content/Views/jobtitle.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForAttendanceReport(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/attendancejs").Include(
                "~/Scripts/Views/Report/report.attendance.js"));
        }

        private static void BuildBundleForUserManagement(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/usermanagementjs").Include(
                "~/Scripts/Views/Employee/user.management.js"));
        }

        private static void BuildBundleForLateInEarlyOut(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lateinearlyoutjs").Include(
                "~/Scripts/Views/Employee/_employee.search.in.company.js",
                "~/Scripts/Views/Report/report.latein.earlyout.js"));
        }

        private static void BuildBundleForLateInEarlyOutDetail(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lateinearlyoutdetailjs")
                .Include("~/Scripts/Views/Employee/_employee.search.in.company.js")
                .Include("~/Scripts/Views/Report/report.latein.earlyout.detail.js"));
        }

        private static void BuildBundleForLateInEarlyOutDiscipline(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lateinearlyoutdisciplinejs").Include(
                "~/Scripts/Views/Employee/_employee.search.in.company.js",
                "~/Scripts/Views/Report/report.latein.earlyout.discipline.js"));

            bundles.Add(new StyleBundle("~/Content/lateinearlyoutdisciplinecss")
                  .Include("~/Content/Views/report.latein.earlyout.discipline.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForEditEmployee(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/employeeeditjs").Include(
                "~/Scripts/Libs/birthdaypicker.js",
                "~/Scripts/Views/Employee/employee.edit.js"));

            bundles.Add(new StyleBundle("~/Content/employeeeditcss")
                  .Include("~/Content/Views/employee.edit.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForTeamManagement(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/teammanagementjs").Include(
                "~/Scripts/Views/TeamManagement/team.management.js"));
        }

        private static void BuildBundleForDefaultSchedule(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/defaultschedulejs").Include(
                "~/Scripts/Views/Employee/employee.defaultschedule.js"));
        }

        private static void BuildBundleForMonthSchedule(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/monthschedulejs").Include(
                "~/Scripts/Views/Employee/employee.monthschedule.js"));
        }

        private static void BuildBundleForReportMyTimeRecord(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/mytimerecordjs").Include(
                "~/Scripts/Views/Report/report.mytimerecord.js"));
        }

        private static void BuildBundleForReportTimeRecord(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/timerecordjs").Include(
                "~/Scripts/Views/Employee/_employee.search.in.company.js",
                "~/Scripts/Views/Report/report.timerecord.js"));
        }

        private static void BuildBundleForLeaveSummary(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/leavesummaryjs").Include(
             "~/Scripts/Views/Employee/_employee.search.in.company.js",
             "~/Scripts/Views/Report/report.leave.summary.js"));
        }

        private static void BuildBundleForDashboard(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/dashboardjs").Include(
                "~/Scripts/Libs/slick.min.js",
                "~/Scripts/Views/Dashboard/employee.dashboard.js",
                "~/Scripts/Views/Dashboard/teamattendance.js",
                "~/Scripts/Libs/moment.js",
                "~/Scripts/Libs/fullcalendar.js",
                "~/Scripts/Libs/gcal.js",
                "~/Scripts/Views/Dashboard/calendar.js",
                "~/Scripts/Views/Dashboard/googleAPI.js",
                "~/Scripts/Views/Dashboard/client.js",
                "~/Scripts/Libs/CustomScrollbar/jquery.mCustomScrollbar.concat.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/dashboard2js").Include(
                "~/Scripts/Views/Dashboard/employee.dashboard.js"));

            bundles.Add(new StyleBundle("~/Content/dashboardcss")
                  .Include("~/Content/Common/vendors/slick.css", new CssRewriteUrlTransform())
                  .Include("~/Content/Views/mydashboard.css", new CssRewriteUrlTransform())
                  .Include("~/Content/Common/vendors/fullcalendar.css", new CssRewriteUrlTransform())
                  .Include("~/Content/Views/mycalendar.css", new CssRewriteUrlTransform())
                  .Include("~/Content/Common/vendors/jquery.mCustomScrollbar.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForWorkingCanlendar(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/workingcalendarsjs").Include(
                "~/Scripts/Views/Calendar/employee.workingcalendars.js",
                "~/Scripts/Views/Employee/_employee.search.in.company.js",
                "~/Scripts/Views/fixtable.js"));

            bundles.Add(new StyleBundle("~/Content/workingcalendarscss")
                  .Include("~/Content/Views/report.timesheet.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForStaffInfo(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/staffinfojs").Include(
                "~/Scripts/Views/Employee/employee.index.js"));
        }

        private static void BuildBundleForStaffBirthday(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/staffbirthdayjs").Include(
                "~/Scripts/Views/Employee/employee.birthday.js"));

            bundles.Add(new StyleBundle("~/Content/staffbirthdaycss")
                  .Include("~/Content/Views/employee.birthday.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForLeaveDeductionDetail(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/leavedeductiondetailjs").Include(
                "~/Scripts/Views/Employee/_employee.search.in.company.js",
                "~/Scripts/Views/AnnualLeave/leave.deduction.detail.js"));
        }

        private static void BuildBundleForLeaveConversionDetail(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/leaveconversiondetailjs").Include(
                "~/Scripts/Views/Employee/_employee.search.in.company.js",
                "~/Scripts/Views/AnnualLeave/leave.conversion.detail.js"));
        }

        private static void BuildBundleForMyProfile(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/myprofilecss")
                  .Include("~/Content/Views/my.profile.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForSimplePagination(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/simplepaginationjs").Include(
                "~/Libs/SimplePagination/jquery.simplePagination.js"));

            bundles.Add(new StyleBundle("~/Content/simplepaginationcss")
                  .Include("~/Libs/SimplePagination/simplePagination.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForFullCalendar(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/fullcalendarjs").Include(
                "~/Scripts/Libs/moment.js",
                "~/Scripts/Libs/fullcalendar.js"));

            bundles.Add(new StyleBundle("~/Content/fullcalendarcss")
                  .Include("~/Content/Common/vendors/fullcalendar.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForContextMenu(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/contextmenujs").Include(
                "~/Scripts/Libs/jquery.contextMenu.min.js"));

            bundles.Add(new StyleBundle("~/Content/contextmenucss")
                  .Include("~/Content/ContextMenu/contextMenu.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForMonthPicker(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/monthpickerjs").Include(
                "~/Scripts/Libs/monPickr-0.0.3.js"));

            bundles.Add(new StyleBundle("~/Content/monthpickercss")
                  .Include("~/Content/Common/vendors/monPickr-0.0.3.css", new CssRewriteUrlTransform()));
        }

        private static void BuildBundleForLateEarly(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lateearlyjs").Include(
                "~/Scripts/Views/LateInEarlyOut/lateinearlyout.js"));

            bundles.Add(new ScriptBundle("~/bundles/createlateearlyjs").Include(
                "~/Scripts/Views/Request/search.request.js",
                "~/Scripts/Views/LateInEarlyOut/create.js"));
        }

        private static void BuildBundleForPostnatal(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/postnatalemployeejs").Include(
                "~/Scripts/Views/Postanal/postnatalemployee.js",
                "~/Scripts/Views/Employee/_employee.search.in.company.js"));

            bundles.Add(new ScriptBundle("~/bundles/createpostnatalemployeejs").Include(
                "~/Scripts/Views/Employee/_employee.search.in.company.js",
                "~/Scripts/Views/Postanal/create.js"));

            bundles.Add(new StyleBundle("~/Content/postnatalcss")
                  .Include("~/Content/Views/employee.postnatal.css", new CssRewriteUrlTransform()));
        }
    }
}