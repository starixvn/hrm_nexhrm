namespace Fanex.NexHR.Services
{
    using Criteria.Calendar;
    using Interfaces;
    using Models.Calendar;
    using Repositories.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CalendarService : ICalendarService
    {
        private readonly ICalendarRepository calendarRepository;

        public CalendarService(ICalendarRepository calendarRepository)
        {
            this.calendarRepository = calendarRepository;
        }

        public async Task<IList<WorkingSchedule>> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            IList<WorkingSchedule> workingSchedules = null;
            if (criteria != null && criteria.IsValid())
            {
                workingSchedules = await calendarRepository.GetWorkingSchedule(criteria);
            }

            return workingSchedules ?? new List<WorkingSchedule>();
        }

        private async Task<IList<LeaveSchedule>> GetLeavingSchedule(GetLeaveScheduleCriteria criteria)
        {
            IList<LeaveSchedule> leaveSchedules = null;
            if (criteria != null && criteria.IsValid())
            {
                leaveSchedules = await calendarRepository.GetLeaveSchedule(criteria);
            }

            return leaveSchedules ?? new List<LeaveSchedule>();
        }

        public async Task<LeaveDaySummarySchedule> GetLeaveSchedule(GetLeaveScheduleCriteria criteria)
        {
            var leaveSummarySchedule = new LeaveDaySummarySchedule();

            if (criteria != null && criteria.IsValid())
            {
                var leaveSchedules = await GetLeavingSchedule(criteria);

                leaveSummarySchedule.TotalLeaves = CalculateTotalLeavesTaken(leaveSchedules);

                var employeeShedules = leaveSchedules.GroupBy(x => x.Employee.Id).Select(g => new EmployeeLeaveSchedule
                {
                    Employee = g.First().Employee,
                    LeaveSchedules = g.OrderBy(e => e.WorkDate).ToList()
                }).ToList();

                leaveSummarySchedule.EmployeeLeaveShedules = ReCalculateEmployeeLeaveSchedule(employeeShedules, GetListDate(criteria.FromDate));
                leaveSummarySchedule.DaysInMonth = DateTime.DaysInMonth(criteria.FromDate.Year, criteria.FromDate.Month);
                leaveSummarySchedule.ListDates = GetListDate(criteria.FromDate);
                leaveSummarySchedule.Month = criteria.FromDate;
            }

            return leaveSummarySchedule;
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

        private IList<double> CalculateTotalLeavesTaken(IList<LeaveSchedule> schedules)
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
                listDates.Add(new DateTime(fromDate.Year, fromDate.Month, i));
            }
            return listDates;
        }
    }
}