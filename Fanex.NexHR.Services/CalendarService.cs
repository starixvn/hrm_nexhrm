namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.Calendar;
    using Interfaces;
    using Models.Calendar;

    public class CalendarService : ICalendarService
    {
        private readonly IBaseService baseService;

        public CalendarService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<LeaveDaySummarySchedule> GetLeaveSchedule(GetLeaveScheduleCriteria criteria)
        {
            var leaveSummarySchedule = new LeaveDaySummarySchedule();

            if (criteria != null && criteria.IsValid())
            {
                var leaveSchedules = await GetLeavingSchedule(criteria);

                leaveSummarySchedule.TotalLeaves = CalculateTotalLeavesTaken(leaveSchedules);

                var employeeShedules = leaveSchedules.GroupBy(x => x.EmpId).Select(g => new EmployeeLeaveSchedule
                {
                    EmpId = g.First().EmpId,
                    NickName = g.First().NickName,
                    FullName = g.First().FullName,
                    TeamCode = g.First().TeamCode,
                    TeamName = g.First().TeamName,
                    DepartmentName = g.First().DepartmentName,
                    LeaveSchedules = g.OrderBy(e => e.WorkDate).ToList()
                }).ToList();

                leaveSummarySchedule.EmployeeLeaveShedules = ReCalculateEmployeeLeaveSchedule(employeeShedules, GetListDate(criteria.FromDate));
                leaveSummarySchedule.DaysInMonth = DateTime.DaysInMonth(criteria.FromDate.Year, criteria.FromDate.Month);
                leaveSummarySchedule.ListDates = GetListDate(criteria.FromDate);
                leaveSummarySchedule.Month = criteria.FromDate;
            }

            return leaveSummarySchedule;
        }

        private async Task<IEnumerable<LeaveSchedule>> GetLeavingSchedule(GetLeaveScheduleCriteria criteria)
        {
            var leaveSchedules = await baseService.GetDataAsync<LeaveSchedule>(criteria);
            return leaveSchedules;
        }

        private IList<EmployeeLeaveSchedule> ReCalculateEmployeeLeaveSchedule(IList<EmployeeLeaveSchedule> schedules, IList<DateTime> listDates)
        {
            foreach (var schedule in schedules)
            {
                if (schedule.LeaveSchedules.Count < listDates.Count)
                {
                    var dateNotContains = listDates.Except(schedule.LeaveSchedules.Select(x => x.WorkDate)).ToList();
                    FillFullWorkDate(dateNotContains, schedule);
                    schedule.LeaveSchedules = schedule.LeaveSchedules.OrderBy(x => x.WorkDate).ToList();
                }
            }
            return schedules;
        }

        private void FillFullWorkDate(IList<DateTime> dateNotContains, EmployeeLeaveSchedule schedule)
        {
            foreach (var date in dateNotContains)
            {
                var leaveSchedule = new LeaveSchedule
                {
                    WorkDate = date
                };
                schedule.LeaveSchedules.Add(leaveSchedule);
            }
        }

        private IList<double> CalculateTotalLeavesTaken(IEnumerable<LeaveSchedule> schedules)
        {
            var totalLeavesTaken = new List<double>();
            var dateLeaves = schedules.GroupBy(x => x.WorkDate).Select(g => new { g.Key, Schedules = g }).OrderBy(g => g.Key);

            foreach (var item in dateLeaves)
            {
                var totalDays = item.Schedules.Sum(x => x.LeaveDay);
                totalLeavesTaken.Add(totalDays);
            }

            return totalLeavesTaken;
        }

        private static IList<DateTime> GetListDate(DateTime fromDate)
        {
            var listDates = new List<DateTime>();
            int daysInMonth = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
            for (int i = 1; i <= daysInMonth; i++)
            {
                listDates.Add(new DateTimeOffset(fromDate.Year, fromDate.Month, i, 0, 0, 0, TimeSpan.Zero).DateTime);
            }
            return listDates;
        }
    }
}