namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Core;
    using Models;
    using Models.Report;

    public class ExportService
    {
        public void ExportTotalWorkingHours(
            IList<TotalWorkingHours> listTotalWorkingHours,
            DateTime fromDate,
            DateTime toDate,
            string templatePath)
        {
            string fileName = "TotalWorkingHours_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year,
                sheetName = "Total Working Hours";

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

        public void ExportEmployeeAttendances(
            IList<EmployeeAttendanceModel> employeeAttendances,
            string templatePath)
        {
            string fileName = "EmployeeAttendance",
                sheetName = "Employee Attendance";

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

        public void ExportEmployeeProfile(
            IList<Employee> employees,
            string templatePath)
        {
            string fileName = "EmployeeProfile",
                sheetName = "Employee Profile";

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

        private void FillContentRowTotalWorkingHours(
            IList<TotalWorkingHours> listTotalWorkingHours,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (TotalWorkingHours totalWorkingHours in listTotalWorkingHours)
            {
                FillRowTotalWorkingHours(totalWorkingHours, ref writer, ref rowIndex);
            }
        }

        private void FillRowTotalWorkingHours(
            TotalWorkingHours totalWorkingHours,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordTotalWorkingHours(totalWorkingHours, ref rowIndex);

            writer.WriteRow((rowIndex - 1) % 2 == 0 ? 5 : 4, record);
        }

        private object CreateRecordTotalWorkingHours(
            TotalWorkingHours totalWorkingHours,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = totalWorkingHours.Employee.Code.Trim(),
                FullName = totalWorkingHours.Employee.FullName,
                Nick = totalWorkingHours.Employee.NickName,
                Team = totalWorkingHours.Employee.Team.Code,
                WorkingDay = totalWorkingHours.WorkingDay,
                ActualWorkingDay = totalWorkingHours.ActualWorkingDay,
                AnnualLeave = totalWorkingHours.AnnualLeave,
                PaidLeave = totalWorkingHours.PaidLeave,
                SickLeave = totalWorkingHours.SickLeave,
                InsuranceLeave = totalWorkingHours.InsuranceLeave,
                UnpaidLeave = totalWorkingHours.UnpaidLeave,
                LateIn = totalWorkingHours.LateIn,
                EarlyOut = totalWorkingHours.EarlyOut,
                TotalHours = totalWorkingHours.TotalHours,
                AverageHours = totalWorkingHours.AverageHours
            };
            rowIndex++;

            return record;
        }

        #endregion Total Working Hours

        #region Employee Attendances

        private void FillContentRowEmployeeAttendances(
            IList<EmployeeAttendanceModel> employeeAttendances,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (EmployeeAttendanceModel employeeAttendance in employeeAttendances)
            {
                FillRowEmployeeAttendances(employeeAttendance, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeAttendances(
            EmployeeAttendanceModel employeeAttendance,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordEmployeeAttendances(employeeAttendance, ref rowIndex);

            writer.WriteRow((rowIndex - 1) % 2 == 0 ? 4 : 3, record);
        }

        private object CreateRecordEmployeeAttendances(
            EmployeeAttendanceModel employeeAttendance,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                AttendanceDate = employeeAttendance.AttendanceDate,
                Code = employeeAttendance.Code,
                Nick = employeeAttendance.NickName,
                FullName = employeeAttendance.FullName,
                Team = employeeAttendance.TeamCode,
                ShiftCode = employeeAttendance.ShiftCode,
                CheckIn = GetTime(employeeAttendance.InFixed),
                CheckOut = GetTime(employeeAttendance.OutFixed),
                TotalHours = TotalHoursToTime(employeeAttendance.TotalWorkHours),
                LateIn = DisplayMinute(employeeAttendance.LateIn),
                EarlyOut = DisplayMinute(employeeAttendance.EarlyOut),
                Notes = employeeAttendance.Notes
            };
            rowIndex++;

            return record;
        }

        #endregion Employee Attendances

        #region Employee Profile

        private void FillContentRowEmployeeProfile(
            IList<Employee> employees,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRowEmployeeProfile(employee, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeProfile(
            Employee employee,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordEmployeeProfile(employee, ref rowIndex);

            writer.WriteRow((rowIndex - 1) % 2 == 0 ? 4 : 3, record);
        }

        private object CreateRecordEmployeeProfile(
            Employee employee,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = employee.Code,
                FullName = employee.FullName,
                Nick = employee.NickName,
                DoB = employee.DateOfBirth,
                Email = employee.Email,
                JobTitle = employee.JobTitle == null ? string.Empty : employee.JobTitle.Name,
                Team = FormatString(employee.Team.Name),
                JoinedDate = employee.JoinedDate,
                ConfirmedDate = employee.ConfirmedDate,
                ResignedDate = employee.ResignedDate,
                Phone = FormatString(employee.Phone),
                Gender = employee.Gender == 0 ? "F" : "M",
                CardNo = FormatString(employee.CardNo),
                IssuedDate = employee.IssuedDate,
                IssuedPlace = FormatString(employee.IssuedPlace),
                CurrentAddress = FormatString(employee.CurrentAddress),
                PermanentAddress = FormatString(employee.PermanentAddress),
                PassportNo = FormatString(employee.PassportNo),
                PassportExp = employee.PassportExp,
                Listening = employee.ToeicScore == null ? string.Empty : FormatToeicScore(employee.ToeicScore.Listening),
                Reading = employee.ToeicScore == null ? string.Empty : FormatToeicScore(employee.ToeicScore.Reading),
                Speaking = employee.ToeicScore == null ? string.Empty : FormatToeicScore(employee.ToeicScore.Speaking),
                Writing = employee.ToeicScore == null ? string.Empty : FormatToeicScore(employee.ToeicScore.Writing)
            };
            rowIndex++;

            return record;
        }

        #endregion Employee Profile

        private string GetTime(DateTime? date)
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
                return timeSpan.ToString("hh:mm");
            }

            return string.Empty;
        }

        private string TotalHoursToTime(decimal value)
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

        private dynamic FormatToeicScore(int? score)
        {
            if (score == null)
            {
                return string.Empty;
            }

            return score.Value;
        }

        private string FormatString(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Trim();
        }

        public void ExportWorkingHoursStatistic(
            IList<WorkingHoursStatistic> listTotalWorkingHours,
            DateTime fromDate,
            DateTime toDate,
            string templatePath)
        {
            string fileName = "WorkingHoursStatistic_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year,
                sheetName = "Total Working Hours " + fromDate.ToString("MMM, yyyy");

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

        private void FillContentRowWorkingHoursStatistic(
            IList<WorkingHoursStatistic> listTotalWorkingHours,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (WorkingHoursStatistic totalWorkingHours in listTotalWorkingHours)
            {
                FillRowWorkingHoursStatistic(totalWorkingHours, ref writer, ref rowIndex);
            }
        }

        private void FillRowWorkingHoursStatistic(
            WorkingHoursStatistic totalWorkingHours,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordWorkingHoursStatistic(totalWorkingHours, ref rowIndex);

            writer.WriteRow((rowIndex - 1) % 2 == 0 ? 4 : 3, record);
        }

        private object CreateRecordWorkingHoursStatistic(
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