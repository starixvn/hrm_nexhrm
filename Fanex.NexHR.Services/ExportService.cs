namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core;
    using Models;
    using Models.Calendar;
    using Models.Report;
    using Resources;

    public static class ExportService
    {
        public static void ExportTotalWorkingHours(
            IList<TotalWorkingHours> listTotalWorkingHours,
            DateTime fromDate,
            DateTime toDate,
            string templatePath)
        {
            string fileName = "TotalWorkingHours_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year;
            string sheetName = "Total Working Hours";

            var reportTitle = "Total Working Hours: " + fromDate.ToString("dd MMM, yyyy") + " - " + toDate.ToString("dd MMM, yyyy");
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                var writer = new ExcelTemplateWriter(fileStream, fileName, sheetName);

                // Fill report title.
                writer.WriteRow(0, new object[] { reportTitle });

                // Fill report header.
                writer.CopyRows(0, 4);
                FillContentRowTotalWorkingHours(listTotalWorkingHours, ref writer);

                writer.CreateFreezePane(0, 4);
                writer.Render();
            }
        }

        public static void ExportEmployeeAttendances(
            IList<EmployeeAttendanceModel> employeeAttendances,
            string templatePath)
        {
            string fileName = "EmployeeAttendance";
            string sheetName = "Employee Attendance";

            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                var writer = new ExcelTemplateWriter(fileStream, fileName, sheetName);

                // Fill report header.
                writer.CopyRows(0, 3);

                FillContentRowEmployeeAttendances(employeeAttendances, ref writer);
                writer.AutosizeColumns(13);
                writer.CreateFreezePane(0, 3);

                writer.Render();
            }
        }

        public static void ExportEmployeeProfile(
            IList<Employee> employees,
            string templatePath)
        {
            string fileName = "EmployeeProfile";
            string sheetName = "Employee Profile";

            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                var writer = new ExcelTemplateWriter(fileStream, fileName, sheetName);

                // Fill report header.
                writer.CopyRows(0, 3);

                FillContentRowEmployeeProfile(employees, ref writer);
                writer.AutosizeColumns(24);
                writer.CreateFreezePane(0, 3);

                writer.Render();
            }
        }

        #region Total Working Hours

        private static void FillContentRowTotalWorkingHours(
            IList<TotalWorkingHours> listTotalWorkingHours,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (TotalWorkingHours totalWorkingHours in listTotalWorkingHours)
            {
                FillRowTotalWorkingHours(totalWorkingHours, ref writer, ref rowIndex);
            }
        }

        private static void FillRowTotalWorkingHours(
            TotalWorkingHours totalWorkingHours,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordTotalWorkingHours(totalWorkingHours, ref rowIndex);

            writer.WriteRow((rowIndex - 1) % 2 == 0 ? 5 : 4, record);
        }

        private static object CreateRecordTotalWorkingHours(
            TotalWorkingHours totalWorkingHours,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = totalWorkingHours.Employee.Code.Trim(),
                totalWorkingHours.Employee.FullName,
                Nick = totalWorkingHours.Employee.NickName,
                Team = totalWorkingHours.Employee.Team.Code,
                totalWorkingHours.WorkingDay,
                totalWorkingHours.ActualWorkingDay,
                totalWorkingHours.AnnualLeave,
                totalWorkingHours.PaidLeave,
                totalWorkingHours.SickLeave,
                totalWorkingHours.InsuranceLeave,
                totalWorkingHours.UnpaidLeave,
                totalWorkingHours.LateIn,
                totalWorkingHours.EarlyOut,
                totalWorkingHours.TotalHours,
                totalWorkingHours.AverageHours
            };
            rowIndex++;

            return record;
        }

        #endregion Total Working Hours

        #region Employee Attendances

        private static void FillContentRowEmployeeAttendances(
            IList<EmployeeAttendanceModel> employeeAttendances,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (EmployeeAttendanceModel employeeAttendance in employeeAttendances)
            {
                FillRowEmployeeAttendances(employeeAttendance, ref writer, ref rowIndex);
            }
        }

        private static void FillRowEmployeeAttendances(
            EmployeeAttendanceModel employeeAttendance,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordEmployeeAttendances(employeeAttendance, ref rowIndex);

            writer.WriteRow((rowIndex - 1) % 2 == 0 ? 4 : 3, record);
        }

        private static object CreateRecordEmployeeAttendances(
            EmployeeAttendanceModel employeeAttendance,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                AttendanceDate = employeeAttendance.AttendanceDate,
                Code = employeeAttendance.Code,
                Nick = employeeAttendance.NickName,
                employeeAttendance.FullName,
                Team = employeeAttendance.TeamCode,
                employeeAttendance.ShiftCode,
                CheckIn = GetTime(employeeAttendance.InFixed),
                CheckOut = GetTime(employeeAttendance.OutFixed),
                TotalHours = TotalHoursToTime(employeeAttendance.TotalWorkHours),
                LateIn = DisplayMinute(employeeAttendance.LateIn),
                EarlyOut = DisplayMinute(employeeAttendance.EarlyOut),
                employeeAttendance.Notes
            };
            rowIndex++;

            return record;
        }

        #endregion Employee Attendances

        #region Employee Profile

        private static void FillContentRowEmployeeProfile(
            IList<Employee> employees,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRowEmployeeProfile(employee, ref writer, ref rowIndex);
            }
        }

        private static void FillRowEmployeeProfile(
            Employee employee,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordEmployeeProfile(employee, ref rowIndex);

            writer.WriteRow((rowIndex - 1) % 2 == 0 ? 4 : 3, record);
        }

        private static object CreateRecordEmployeeProfile(
            Employee employee,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                employee.Code,
                employee.FullName,
                Nick = employee.NickName,
                DoB = employee.DateOfBirth,
                employee.Email,
                JobTitle = JobTitle(employee),
                Team = FormatString(employee.Team.Name),
                JoinedDate = employee.JoinedDate,
                ConfirmedDate = employee.ConfirmedDate,
                ResignedDate = employee.ResignedDate,
                Phone = FormatString(employee.Phone),
                Gender = Gender(employee),
                CardNo = FormatString(employee.CardNo),
                IssuedDate = employee.IssuedDate,
                IssuedPlace = FormatString(employee.IssuedPlace),
                CurrentAddress = FormatString(employee.CurrentAddress),
                PermanentAddress = FormatString(employee.PermanentAddress),
                PassportNo = FormatString(employee.PassportNo),
                PassportExp = employee.PassportExp,
                Listening = FormatToeicScore(employee.ToeicScore?.Listening),
                Reading = FormatToeicScore(employee.ToeicScore?.Reading),
                Speaking = FormatToeicScore(employee.ToeicScore?.Speaking),
                Writing = FormatToeicScore(employee.ToeicScore?.Writing)
            };
            rowIndex++;

            return record;
        }

        private static string JobTitle(Employee employee)
        {
            return employee.JobTitle == null ? string.Empty : employee.JobTitle.Name;
        }

        private static string Gender(Employee employee)
        {
            return employee.Gender == 0 ? "F" : "M";
        }

        #endregion Employee Profile

        #region Employee Leave Schedule

        public static void ExportLeaveSummaryScheduleReport(LeaveDaySummarySchedule schedules, string templatePath)
        {
            string fileName = Labels.LeaveTakenSchedulesReport;
            string sheetName = Labels.LeaveSchedules;

            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                var writer = new ExcelTemplateWriter(fileStream, fileName, sheetName);

                writer.CopyRows(0, 1);
                writer.CopyRows(1, 3);
                WriteHeaderLeaveDaysSchedule(schedules, ref writer);
                WriteLeaveDayScheduleContent(schedules.EmployeeLeaveShedules, ref writer);
                WriteLeaveDayScheduleSumRow(schedules.TotalLeaves, ref writer, schedules.EmployeeLeaveShedules.Count);

                writer.SyncColumnsWidth(35);
                writer.Render();
            }
        }

        private static void WriteHeaderLeaveDaysSchedule(LeaveDaySummarySchedule schedules, ref ExcelTemplateWriter writer)
        {
            var startCol = 4;
            writer.WriteCellRange(1, 1, startCol, schedules.DaysInMonth, schedules.Month.ToString("MMM yyyy"));
            foreach (var date in schedules.ListDates)
            {
                writer.WriteCellRange(3, 1, startCol, 1, date.ToString("ddd"));
                startCol++;
            }
        }

        private static void WriteLeaveDayScheduleContent(IList<EmployeeLeaveSchedule> schedules, ref ExcelTemplateWriter writer)
        {
            var count = 0;
            var rowIndex = 4;
            foreach (var schedule in schedules)
            {
                count++;
                var employeeInfo = new
                {
                    No = count,
                    Name = schedule.Employee.FullName,
                    Nick = schedule.Employee.NickName,
                    TeamName = schedule.Employee.CodeOfTeam
                };
                writer.WriteRow(4, employeeInfo);
                WriteDateLeaveSchedules(schedule.LeaveSchedules, ref writer, rowIndex);
                rowIndex++;
            }
        }

        private static void WriteDateLeaveSchedules(IList<LeaveSchedule> leaveSchedules, ref ExcelTemplateWriter writer, int rowIndex)
        {
            var startCol = 4;
            foreach (var leaveSchedule in leaveSchedules)
            {
                var cellValue = leaveSchedule.IsLeaveDay ? !leaveSchedule.IsHalfDayLeave ? "x" : leaveSchedule.LeaveType : leaveSchedule.LeaveType;

                writer.WriteCellRange(rowIndex, 1, startCol, 1, cellValue);
                startCol++;
            }
        }

        private static void WriteLeaveDayScheduleSumRow(IList<double> totalLeaves, ref ExcelTemplateWriter writer, int employeeLength)
        {
            writer.CopyRows(5, 1);
            var rowIndex = employeeLength + 4;
            var startCol = 4;
            foreach (var totalLeave in totalLeaves)
            {
                writer.WriteCellRange(rowIndex, 1, startCol, 1, totalLeave);
                startCol++;
            }
        }

        #endregion Employee Leave Schedule

        #region Head Count Report

        public static void ExportHeadCountReport(HeadCountModel report, string templatePath)
        {
            var reportName = string.Format("{0}{1}", "HeadCount", report.StartDate.Year);
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                var writer = new ExcelTemplateWriter(fileStream, reportName, reportName);
                // Fill report header.
                writer.WriteRow(0, new object[] { string.Format("{0}{1}", "HEAD COUNT ", report.StartDate.Year) });
                FillHeaderHeadCount(report, ref writer);
                FillContentHeadCount(report.HeadCount, ref writer);
                FillTotalRowHeadCount(report.HeadCount, ref writer);
                writer.AutosizeColumns(8);

                writer.Render();
            }
        }

        private static void FillHeaderHeadCount(HeadCountModel report, ref ExcelTemplateWriter writer)
        {
            var row = new
            {
                Count = "No.",
                Team = "Team",
                Code = "Code",
                FirstDate = report.StartDate.ToString("dd MMM yyyy"),
                NewHire = Labels.NewHire,
                Resignation = Labels.Resignation,
                LastDate = report.EndDate.ToString("dd MMM yyyy"),
                Variance = Labels.Variance
            };
            writer.WriteRow(1, row);
        }

        private static void FillContentHeadCount(IList<HeadCount> headCount, ref ExcelTemplateWriter writer)
        {
            var index = 1;
            foreach (HeadCount item in headCount)
            {
                var record = new
                {
                    Count = index,
                    Team = item.GroupName,
                    Code = item.GroupCode,
                    FirstDate = item.EmployeeAtJanuary,
                    NewHire = item.NewHire != 0 ? item.NewHire.ToString() : string.Empty,
                    Resignation = item.Resigned != 0 ? item.Resigned.ToString() : string.Empty,
                    LastDate = item.EmployeeAtDecember,
                    Variance = item.Variance
                };
                writer.WriteRow(2, record);
                index++;
            }
        }

        private static void FillTotalRowHeadCount(IList<HeadCount> headCount, ref ExcelTemplateWriter writer)
        {
            var record = new
            {
                Total = Labels.Total,
                MergeCol1 = string.Empty,
                MergeCol2 = string.Empty,
                EmployeeAtJanuary = headCount.Sum(x => x.EmployeeAtJanuary),
                NewHire = headCount.Sum(x => x.NewHire),
                Resigned = headCount.Sum(x => x.Resigned),
                EmployeeAtDecember = headCount.Sum(x => x.EmployeeAtDecember),
                Variance = headCount.Sum(x => x.Variance)
            };
            writer.WriteRow(3, record);
        }

        #endregion Head Count Report

        private static string GetTime(DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }

            return date.Value.ToString("hh:mm");
        }

        private static string DisplayMinute(int minutes)
        {
            if (minutes > 0)
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(minutes);
                return $"{timeSpan.Hours}:{timeSpan.Minutes}";
            }

            return string.Empty;
        }

        private static string TotalHoursToTime(decimal value)
        {
            if (value > 0)
            {
                var display = string.Empty;
                var hour = Math.Floor(value);
                if (hour > 0)
                {
                    display = hour < 10 ? "0" + hour + "h" : hour + "h";
                }

                var minute = Math.Round((value - hour) * 60);
                return minute < 10 ? display + "0" + minute + "'" : display + minute + "'";
            }

            return string.Empty;
        }

        private static dynamic FormatToeicScore(int? score)
        {
            if (score == null)
            {
                return string.Empty;
            }

            return score.Value;
        }

        private static string FormatString(string value)
        {
            return value?.Trim() ?? string.Empty;
        }

        public static void ExportWorkingHoursStatistic(
            IList<WorkingHoursStatistic> listTotalWorkingHours,
            DateTime fromDate,
            DateTime toDate,
            string templatePath)
        {
            string fileName = "WorkingHoursStatistic_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year;
            string sheetName = "Total Working Hours " + fromDate.ToString("MMM, yyyy");

            var reportTitle = "Total Working Hours: " + fromDate.ToString("dd MMM, yyyy") + " - " + toDate.ToString("dd MMM, yyyy");
            using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                var writer = new ExcelTemplateWriter(fileStream, fileName, sheetName);

                // Fill report title.
                writer.WriteRow(0, new object[] { reportTitle });

                // Fill report header.
                writer.CopyRows(0, 3);
                FillContentRowWorkingHoursStatistic(listTotalWorkingHours, ref writer);

                writer.CreateFreezePane(0, 4);
                writer.Render();
            }
        }

        #region Working Hours Statistic

        private static void FillContentRowWorkingHoursStatistic(
            IList<WorkingHoursStatistic> listTotalWorkingHours,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (WorkingHoursStatistic totalWorkingHours in listTotalWorkingHours)
            {
                FillRowWorkingHoursStatistic(totalWorkingHours, ref writer, ref rowIndex);
            }
        }

        private static void FillRowWorkingHoursStatistic(
            WorkingHoursStatistic totalWorkingHours,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordWorkingHoursStatistic(totalWorkingHours, ref rowIndex);

            writer.WriteRow((rowIndex - 1) % 2 == 0 ? 4 : 3, record);
        }

        private static object CreateRecordWorkingHoursStatistic(
            WorkingHoursStatistic totalWorkingHours,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = totalWorkingHours.Employee.Code.Trim(),
                FullName = totalWorkingHours.Employee.FullName,
                Nick = totalWorkingHours.Employee.NickName,
                Team = totalWorkingHours.Employee.Team.Code,
                ActualWorkingDay = totalWorkingHours.ActualWorkingDay,
                AnnualLeave = totalWorkingHours.AnnualLeave,
                PaidLeave = totalWorkingHours.PaidLeave,
                SickLeave = totalWorkingHours.SickLeave,
                InsuranceLeave = totalWorkingHours.InsuranceLeave,
                UnpaidLeave = totalWorkingHours.UnpaidLeave,
                WorkingDay = totalWorkingHours.TotalWorkingDay,
                AnnualLeaveBalance = 0,
                PreMonthSickLeave = totalWorkingHours.PreMonthSickLeave,
                PreMonthUnpaidLeave = totalWorkingHours.PreMonthUnpaidLeave
            };
            rowIndex++;

            return record;
        }

        #endregion Working Hours Statistic
    }
}