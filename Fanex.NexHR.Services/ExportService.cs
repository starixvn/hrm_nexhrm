namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Services.ExportExcel;
    using Models;
    using Models.Calendar;
    using Models.Report;

    public static class ExportService
    {
        public static byte[] ExportTotalWorkingHours(
            IList<TotalWorkingHours> listTotalWorkingHours,
            DateTime fromDate,
            DateTime toDate,
            string templatePath)
        {
            var excelTotalWorkingHours = new TotalWorkingHoursExcel(templatePath, listTotalWorkingHours, fromDate, toDate);
            return excelTotalWorkingHours.Export();
        }

        public static byte[] ExportEmployeeAttendances(
            IList<EmployeeAttendanceModel> employeeAttendances,
            string templatePath)
        {
            var excel = new AttendancesReportExcel(templatePath, employeeAttendances);
            return excel.Export();
        }

        public static byte[] ExportEmployeeProfile(
            IList<Employee> employees,
            string templatePath)
        {
            var employeeProfileExcel = new EmployeeProfileExcel(templatePath, employees);
            return employeeProfileExcel.Export();
        }

        public static byte[] ExportLeaveSummaryScheduleReport(LeaveDaySummarySchedule schedules, string templatePath)
        {
            var leaveScheduleExcel = new LeaveScheduleExcel(templatePath, schedules);
            return leaveScheduleExcel.Export();
        }

        public static byte[] ExportHeadCountReport(HeadCountModel report, string templatePath)
        {
            var headCountExcel = new HeadCountReportExcel(templatePath, report, report.StartDate.Year);
            return headCountExcel.Export();
        }

        public static byte[] ExportWorkingHoursStatistic(
            IList<WorkingHoursStatistic> listTotalWorkingHours,
            DateTime fromDate,
            DateTime toDate,
            string templatePath)
        {
            var excelWorkingHoursStatistic = new WorkingHoursStatisticExcel(templatePath, listTotalWorkingHours, fromDate, toDate);
            return excelWorkingHoursStatistic.Export();
        }
    }
}