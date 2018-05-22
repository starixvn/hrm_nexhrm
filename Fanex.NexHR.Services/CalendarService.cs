namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.Calendar;
    using Interfaces;
    using Models.Calendar;
    using Repositories;
    using Repositories.Interfaces;

    public class CalendarService : BaseService, ICalendarService
    {
        private readonly ICalendarRepository _calendarRepository;

        public CalendarService(ICalendarRepository calendarRepository = null)
        {
            _calendarRepository = calendarRepository ?? new CalendarRepository();
        }

        public IList<WorkingSchedule> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            IList<WorkingSchedule> workingSchedules = null;
            if (criteria != null && criteria.IsValid())
            {
                workingSchedules = _calendarRepository.GetWorkingSchedule(criteria);
            }

            return workingSchedules ?? new List<WorkingSchedule>();
        }

        private IList<LeaveSchedule> GetLeavingSchedule(GetLeaveScheduleCriteria criteria)
        {
            IList<LeaveSchedule> leaveSchedules = null;
            if (criteria != null && criteria.IsValid())
            {
                leaveSchedules = _calendarRepository.GetLeaveSchedule(criteria);
            }

            return leaveSchedules ?? new List<LeaveSchedule>();
        }

        public LeaveDaySummarySchedule GetLeaveSchedule(GetLeaveScheduleCriteria criteria)
        {
            var leaveSummarySchedule = new LeaveDaySummarySchedule();

            if (criteria != null && criteria.IsValid())
            {
                var leaveSchedules = GetLeavingSchedule(criteria);

                var employeeShedules = leaveSchedules.GroupBy(x => x.Employee.Id).Select(g => new EmployeeLeaveSchedule
                {
                    Employee = g.First().Employee,
                    LeaveSchedules = g.OrderBy(e => e.WorkDate).ToList()
                }).ToList();
                leaveSummarySchedule.TotalLeaves = CalculateTotalLeavesTaken(leaveSchedules);
                leaveSummarySchedule.EmployeeLeaveShedules = employeeShedules;
                leaveSummarySchedule.DaysInMonth = DateTime.DaysInMonth(criteria.FromDate.Year, criteria.FromDate.Month);
                leaveSummarySchedule.ListDates = GetListDate(criteria.FromDate, criteria.ToDate);
                leaveSummarySchedule.Month = criteria.FromDate;
            }

            return leaveSummarySchedule;
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

        private static IList<DateTime> GetListDate(DateTime fromDate, DateTime toDate)
        {
            var listDates = new List<DateTime>();
            foreach (DateTime day in GetDatesFromRange(fromDate, toDate))
            {
                listDates.Add(day);
            }
            return listDates;
        }

        private static IEnumerable<DateTime> GetDatesFromRange(DateTime fromDate, DateTime toDate)
        {
            for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }
    }
}