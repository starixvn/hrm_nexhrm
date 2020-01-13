namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core;
    using Core.Enums;
    using Models;
    using Report;

    public class MySpaceViewModel
    {
        private const float numberEqual = 0.5f;
        private const int numberLength = 2;
        private const int year = 2000;

        private const string DisplayCellTpl =
            @"<span class='contextmenu'><a href='javascript:;' class='blue-grey-text text-darken-3'>{0}</a>
            <ul class='list-unstyled drop-menu'>{1}</ul></span>";

        private const string DisplayCellDetailTpl =
            "<li><span class='mdi-navigation-chevron-right'></span><span>{0}</span></li>";

        public decimal AnnualLeaveBalance { get; set; }

        public decimal TotalWorkingHoursToday { get; set; }

        public decimal TotalWorkingHoursCurrentMonth { get; set; }

        public decimal TotalWorkingHoursLastMonth { get; set; }

        public decimal NumberWorkingDayCurrentMonth { get; set; }

        public decimal NumberWorkingDayLastMonth { get; set; }

        public DateTime CheckInTime { get; set; }

        public DateTime CheckOutTime { get; set; }

        public int MinutesLateInEarlyOutCurrentMonth { get; set; }

        public int TimesLateInEarlyOutCurrentMonth { get; set; }

        public bool IsLoadTeamWorkload { get; set; }

        public IList<DashBoardCompanySpace> DashboardCompanySpace { get; set; }

        public IList<DashBoardPersonalSpace> DashboardEmployeeSpace { get; set; }

        public string CompanyLeaveToDay
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> todayLeaves = DashboardCompanySpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Leave && t.IsCurrent))
                    .OrderByDescending(t => t.LeaveDay)
                    .ThenBy(t => t.Employee.NickName)
                    .ToList();

                foreach (DashBoardCompanySpace leave in todayLeaves)
                {
                    employees.Append(leave.Employee.NickName);
                    employees.Append(leave.LeaveDay.Equals(numberEqual) ? " (½)" : string.Empty);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString().Substring(0, employees.Length - numberLength) : string.Empty;
            }
        }

        public string CompanyLeaveYesterday
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> yesterdayLeaves = DashboardCompanySpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Leave && !t.IsCurrent))
                    .OrderByDescending(t => t.LeaveDay)
                    .ThenBy(t => t.Employee.NickName)
                    .ToList();
                foreach (DashBoardCompanySpace leave in yesterdayLeaves)
                {
                    employees.Append(leave.Employee.NickName);
                    employees.Append(leave.LeaveDay.Equals(numberEqual) ? " (½)" : string.Empty);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString().Substring(0, employees.Length - numberLength) : string.Empty;
            }
        }

        public string CompanyCheckInOutToDay
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> todayCheckInOuts = DashboardCompanySpace
                    .Where(t => (t.TypeId == (int)DashBoardType.CheckInOut && t.IsCurrent))
                    .ToList();
                foreach (DashBoardCompanySpace checkInOut in todayCheckInOuts)
                {
                    employees.Append(checkInOut.Employee.NickName);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString(0, employees.Length - numberLength) : employees.ToString();
            }
        }

        public string CompanyCheckInOutYesterday
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> yesterdayCheckInOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.CheckInOut && !t.IsCurrent))
                        .ToList();
                foreach (DashBoardCompanySpace checkInOut in yesterdayCheckInOuts)
                {
                    employees.Append(checkInOut.Employee.NickName);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString(0, employees.Length - numberLength) : employees.ToString();
            }
        }

        public string CompanyMissingCheckInOutToDay
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> todayMissingCheckInOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.MissingCheckInOut && t.IsCurrent))
                        .ToList();
                foreach (DashBoardCompanySpace checkInOut in todayMissingCheckInOuts)
                {
                    employees.Append(checkInOut.Employee.NickName);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString(0, employees.Length - numberLength) : employees.ToString();
            }
        }

        public string CompanyMissingCheckInOutYesterday
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> yesterdayMissingCheckInOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.MissingCheckInOut && !t.IsCurrent))
                        .ToList();
                foreach (DashBoardCompanySpace checkInOut in yesterdayMissingCheckInOuts)
                {
                    employees.Append(checkInOut.Employee.NickName);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString(0, employees.Length - numberLength) : employees.ToString();
            }
        }

        public string CompanyLateInEarlyOutToDay
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> todayLateInEarlyOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.LateInEarlyOut && t.IsCurrent))
                        .ToList();
                foreach (DashBoardCompanySpace checkInOut in todayLateInEarlyOuts)
                {
                    employees.Append(checkInOut.Employee.NickName);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString(0, employees.Length - numberLength) : employees.ToString();
            }
        }

        public string CompanyLateInEarlyOutYesterday
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> yesterdayLateInEarlyOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.LateInEarlyOut && !t.IsCurrent))
                        .ToList();
                foreach (DashBoardCompanySpace checkInOut in yesterdayLateInEarlyOuts)
                {
                    employees.Append(checkInOut.Employee.NickName);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString(0, employees.Length - numberLength) : employees.ToString();
            }
        }

        public string CompanyAbsentToDay
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> todayAbsents = DashboardCompanySpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Absent && t.IsCurrent))
                    .ToList();
                foreach (DashBoardCompanySpace checkInOut in todayAbsents)
                {
                    employees.Append(checkInOut.Employee.NickName);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString(0, employees.Length - numberLength) : employees.ToString();
            }
        }

        public string CompanyAbsentYesterday
        {
            get
            {
                var employees = new StringBuilder();
                IList<DashBoardCompanySpace> yesterdayAbsents = DashboardCompanySpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Absent && !t.IsCurrent))
                    .ToList();
                foreach (DashBoardCompanySpace checkInOut in yesterdayAbsents)
                {
                    employees.Append(checkInOut.Employee.NickName);
                    employees.Append(", ");
                }

                return employees.Length > numberLength ? employees.ToString(0, employees.Length - numberLength) : employees.ToString();
            }
        }

        public string PersonalLeaveCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthLeaves = DashboardEmployeeSpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Leave && t.IsCurrent))
                    .ToList();

                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthLeaves.Count > 0)
                {
                    foreach (DashBoardPersonalSpace leave in currentMonthLeaves)
                    {
                        dates.Append(string.Format(DisplayCellDetailTpl, leave.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, currentMonthLeaves.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalLeaveLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthLeaves = DashboardEmployeeSpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Leave && !t.IsCurrent))
                    .ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthLeaves.Count > 0)
                {
                    foreach (DashBoardPersonalSpace leave in lastMonthLeaves)
                    {
                        dates.Append(string.Format(DisplayCellDetailTpl, leave.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, lastMonthLeaves.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalCheckInOutCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthCheckInOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.CheckInOut && t.IsCurrent))
                        .ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthCheckInOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace checkInOut in currentMonthCheckInOuts)
                    {
                        dates.Append(string.Format(
                            DisplayCellDetailTpl,
                            checkInOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, currentMonthCheckInOuts.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalCheckInOutLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthCheckInOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.CheckInOut && !t.IsCurrent))
                        .ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthCheckInOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace checkInOut in lastMonthCheckInOuts)
                    {
                        dates.Append(string.Format(
                            DisplayCellDetailTpl,
                            checkInOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, lastMonthCheckInOuts.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalMissingCheckInOutCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthMissingCheckInOuts =
                    DashboardEmployeeSpace.Where(t => (
                            (t.TypeId == (int)DashBoardType.MissingCheckInOut
                             || t.TypeId == (int)DashBoardType.CheckInOut)
                            && t.IsCurrent))
                        .ToList();

                var dates = new StringBuilder();
                var displayValue = "-";

                if (currentMonthMissingCheckInOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace missingCheckInOut in currentMonthMissingCheckInOuts)
                    {
                        dates.Append(string.Format(
                            DisplayCellDetailTpl,
                            missingCheckInOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, currentMonthMissingCheckInOuts.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalMissingCheckInOutLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthMissingCheckInOuts =
                    DashboardEmployeeSpace
                        .Where(t => (t.TypeId == (int)DashBoardType.MissingCheckInOut && !t.IsCurrent))
                        .ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthMissingCheckInOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace missingCheckInOut in lastMonthMissingCheckInOuts)
                    {
                        dates.Append(string.Format(
                            DisplayCellDetailTpl,
                            missingCheckInOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, lastMonthMissingCheckInOuts.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalLateInEarlyOutCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthLateInEarlyOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.LateInEarlyOut && t.IsCurrent))
                        .ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthLateInEarlyOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace lateInEarlyOut in currentMonthLateInEarlyOuts)
                    {
                        dates.Append(string.Format(
                            DisplayCellDetailTpl,
                            lateInEarlyOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, currentMonthLateInEarlyOuts.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalLateInEarlyOutLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthLateInEarlyOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.LateInEarlyOut && !t.IsCurrent))
                        .ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthLateInEarlyOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace lateInEarlyOut in lastMonthLateInEarlyOuts)
                    {
                        dates.Append(string.Format(
                            DisplayCellDetailTpl,
                            lateInEarlyOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, lastMonthLateInEarlyOuts.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalAbsentCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthAbsents = DashboardEmployeeSpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Absent && t.IsCurrent))
                    .ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthAbsents.Count > 0)
                {
                    foreach (DashBoardPersonalSpace absent in currentMonthAbsents)
                    {
                        dates.Append(string.Format(
                            DisplayCellDetailTpl,
                            absent.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, currentMonthAbsents.Count, dates);
                }

                return displayValue;
            }
        }

        public string PersonalAbsentLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthAbsents = DashboardEmployeeSpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Absent && !t.IsCurrent))
                    .ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthAbsents.Count > 0)
                {
                    foreach (DashBoardPersonalSpace absent in lastMonthAbsents)
                    {
                        dates.Append(string.Format(
                            DisplayCellDetailTpl,
                            absent.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(DisplayCellTpl, lastMonthAbsents.Count, dates);
                }

                return displayValue;
            }
        }

        public string DisplayCheckInTime
        {
            get
            {
                var displayValue = "None";

                if ((CheckInTime - new DateTime(year, 1, 1)).Days != 0)
                {
                    displayValue = CheckInTime.ToShortTimeString();
                }

                return displayValue;
            }
        }

        public string DisplayCheckOutTime
        {
            get
            {
                var displayValue = "None";

                if ((CheckOutTime - new DateTime(year, 1, 1)).Days != 0)
                {
                    displayValue = CheckOutTime.ToShortTimeString();
                }

                return displayValue;
            }
        }

        public string DisplayTotalWorkingHoursToday
        {
            get
            {
                var displayValue = "--:--";

                if (TotalWorkingHoursToday != 0)
                {
                    return Utilities.DecimalToTime(TotalWorkingHoursToday);
                }

                return displayValue;
            }
        }

        public SystemReminderModel SystemReminder { get; set; }

        public EmployeeAttendanceViewModel EmployeeAttendanceModel { get; set; }

        public IList<LateInEarlyOut.LateInEarlyOutViewModel> NotificationLateInEarlyOutToDay { get; set; }

        public string DisplayLateInEarlyOutNotification
        {
            get
            {
                var results = new StringBuilder();

                if (NotificationLateInEarlyOutToDay != null)
                {
                    foreach (var item in NotificationLateInEarlyOutToDay)
                    {
                        string itemString =
                            $"{item.Nickname}<span class=\'txt-normal\'> ({item.Types} {item.Minutes}\')</span>, ";
                        results.Append(itemString);
                    }
                }

                return results.Length > numberLength ? results.ToString(0, results.Length - numberLength) : results.ToString();
            }
        }

        public static string RandomPictureName
        {
            get
            {
                Random randomPicture = new Random();
                string picture = randomPicture.Next(1, 5) + ".png";
                return picture;
            }
        }
    }
}