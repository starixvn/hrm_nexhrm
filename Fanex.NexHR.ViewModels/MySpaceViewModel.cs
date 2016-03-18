namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels.Report;

    public class MySpaceViewModel
    {
        private const string _DisplayCellTpl = @"<span class='contextmenu'><a href='javascript:;' class='blue-grey-text text-darken-3'>{0}</a>
            <ul class='list-unstyled drop-menu'>{1}</ul></span>";

        private const string _DisplayCellDetailTpl = "<li><span class='mdi-navigation-chevron-right'></span><span>{0}</span></li>";

        public decimal AnnualLeaveBalance { get; set; }

        public decimal TotalWorkingHoursToday { get; set; }

        public decimal TotalWorkingHoursCurrentMonth { get; set; }

        public decimal TotalWorkingHoursLastMonth { get; set; }

        public decimal NumberWorkingDayCurrentMonth { get; set; }

        public decimal NumberWorkingDayLastMonth { get; set; }

        public DateTime CheckInTime { get; set; }

        public DateTime CheckOutTime { get; set; }

        public IList<DashBoardCompanySpace> DashboardCompanySpace { get; set; }

        public IList<DashBoardPersonalSpace> DashboardEmployeeSpace { get; set; }

        public string CompanyLeaveToDay
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> todayLeaves = DashboardCompanySpace
                    .Where(t => (t.TypeId == (int)DashBoardType.Leave && t.IsCurrent))
                    .OrderByDescending(t => t.LeaveDay)
                    .ThenBy(t => t.Employee.NickName).ToList();
                foreach (DashBoardCompanySpace leave in todayLeaves)
                {
                    employees += leave.Employee.NickName + (leave.LeaveDay == 0.5 ? " (½)" : string.Empty) + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyLeaveYesterday
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> yesterdayLeaves = DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.Leave && !t.IsCurrent))
                    .OrderByDescending(t => t.LeaveDay).ThenBy(t => t.Employee.NickName).ToList();
                foreach (DashBoardCompanySpace leave in yesterdayLeaves)
                {
                    employees += leave.Employee.NickName + (leave.LeaveDay == 0.5 ? " (½)" : string.Empty) + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyCheckInOutToDay
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> todayCheckInOuts = DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.CheckInOut && t.IsCurrent)).ToList();
                foreach (DashBoardCompanySpace checkInOut in todayCheckInOuts)
                {
                    employees += checkInOut.Employee.NickName + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyCheckInOutYesterday
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> yesterdayCheckInOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.CheckInOut && !t.IsCurrent)).ToList();
                foreach (DashBoardCompanySpace checkInOut in yesterdayCheckInOuts)
                {
                    employees += checkInOut.Employee.NickName + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyMissingCheckInOutToDay
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> todayMissingCheckInOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.MissingCheckInOut && t.IsCurrent)).ToList();
                foreach (DashBoardCompanySpace missingCheckInOut in todayMissingCheckInOuts)
                {
                    employees += missingCheckInOut.Employee.NickName + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyMissingCheckInOutYesterday
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> yesterdayMissingCheckInOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.MissingCheckInOut && !t.IsCurrent)).ToList();
                foreach (DashBoardCompanySpace missingCheckInOut in yesterdayMissingCheckInOuts)
                {
                    employees += missingCheckInOut.Employee.NickName + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyLateInEarlyOutToDay
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> todayLateInEarlyOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.LateInEarlyOut && t.IsCurrent)).ToList();
                foreach (DashBoardCompanySpace lateInEarlyOut in todayLateInEarlyOuts)
                {
                    employees += lateInEarlyOut.Employee.NickName + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyLateInEarlyOutYesterday
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> yesterdayLateInEarlyOuts =
                    DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.LateInEarlyOut && !t.IsCurrent)).ToList();
                foreach (DashBoardCompanySpace lateInEarlyOut in yesterdayLateInEarlyOuts)
                {
                    employees += lateInEarlyOut.Employee.NickName + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyAbsentToDay
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> todayAbsents = DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.Absent && t.IsCurrent)).ToList();
                foreach (DashBoardCompanySpace absent in todayAbsents)
                {
                    employees += absent.Employee.NickName + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string CompanyAbsentYesterday
        {
            get
            {
                var employees = string.Empty;
                IList<DashBoardCompanySpace> yesterdayAbsents = DashboardCompanySpace.Where(t => (t.TypeId == (int)DashBoardType.Absent && !t.IsCurrent)).ToList();
                foreach (DashBoardCompanySpace absent in yesterdayAbsents)
                {
                    employees += absent.Employee.NickName + ", ";
                }

                if (employees.Length > 2)
                {
                    employees = employees.Substring(0, employees.Length - 2);
                }

                return employees;
            }
        }

        public string PersonalLeaveCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthLeaves = DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.Leave && t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthLeaves.Count > 0)
                {
                    foreach (DashBoardPersonalSpace leave in currentMonthLeaves)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, leave.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, currentMonthLeaves.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalLeaveLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthLeaves = DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.Leave && !t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthLeaves.Count > 0)
                {
                    foreach (DashBoardPersonalSpace leave in lastMonthLeaves)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, leave.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, lastMonthLeaves.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalCheckInOutCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthCheckInOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.CheckInOut && t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthCheckInOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace checkInOut in currentMonthCheckInOuts)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, checkInOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, currentMonthCheckInOuts.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalCheckInOutLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthCheckInOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.CheckInOut && !t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthCheckInOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace checkInOut in lastMonthCheckInOuts)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, checkInOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, lastMonthCheckInOuts.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalMissingCheckInOutCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthMissingCheckInOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.MissingCheckInOut && t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthMissingCheckInOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace missingCheckInOut in currentMonthMissingCheckInOuts)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, missingCheckInOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, currentMonthMissingCheckInOuts.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalMissingCheckInOutLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthMissingCheckInOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.MissingCheckInOut && !t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthMissingCheckInOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace missingCheckInOut in lastMonthMissingCheckInOuts)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, missingCheckInOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, lastMonthMissingCheckInOuts.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalLateInEarlyOutCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthLateInEarlyOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.LateInEarlyOut && t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthLateInEarlyOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace lateInEarlyOut in currentMonthLateInEarlyOuts)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, lateInEarlyOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, currentMonthLateInEarlyOuts.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalLateInEarlyOutLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthLateInEarlyOuts =
                    DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.LateInEarlyOut && !t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthLateInEarlyOuts.Count > 0)
                {
                    foreach (DashBoardPersonalSpace lateInEarlyOut in lastMonthLateInEarlyOuts)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, lateInEarlyOut.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, lastMonthLateInEarlyOuts.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalAbsentCurrentMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> currentMonthAbsents = DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.Absent && t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (currentMonthAbsents.Count > 0)
                {
                    foreach (DashBoardPersonalSpace absent in currentMonthAbsents)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, absent.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, currentMonthAbsents.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string PersonalAbsentLastMonth
        {
            get
            {
                IList<DashBoardPersonalSpace> lastMonthAbsents = DashboardEmployeeSpace.Where(t => (t.TypeId == (int)DashBoardType.Absent && !t.IsCurrent)).ToList();
                var dates = new StringBuilder();
                var displayValue = "0";
                if (lastMonthAbsents.Count > 0)
                {
                    foreach (DashBoardPersonalSpace absent in lastMonthAbsents)
                    {
                        dates.Append(string.Format(_DisplayCellDetailTpl, absent.WorkDate.ToDateMonthYearFormatedDate()));
                    }

                    displayValue = string.Format(_DisplayCellTpl, lastMonthAbsents.Count, dates.ToString());
                }

                return displayValue;
            }
        }

        public string DisplayCheckInTime
        {
            get
            {
                var displayValue = "None";

                if ((CheckInTime - new DateTime(2000, 1, 1)).Days != 0)
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

                if ((CheckOutTime - new DateTime(2000, 1, 1)).Days != 0)
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
    }
}