namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Calendar;

    public class CalendarRepository : ICalendarRepository
    {
        public async Task<IList<WorkingSchedule>> GetWorkingSchedule(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var workingSchedules = await db.QueryAsync<WorkingSchedule, Employee, Team, WorkingSchedule>(
                     (schedule, employee, team) =>
                     {
                         employee.Team = team;
                         schedule.Employee = employee;
                         return schedule;
                     },
                     "EmpId, GroupId",
                     criteria);

                return workingSchedules.ToList();
            }
        }

        public async Task<IList<LeaveSchedule>> GetLeaveSchedule(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var leaveSchedules = await db.QueryAsync<LeaveSchedule, Employee, Team, LeaveSchedule>(
                     (schedule, employee, team) =>
                     {
                         employee.Team = team;
                         schedule.Employee = employee;
                         return schedule;
                     },
                     "EmpId, GroupId",
                     criteria);

                return leaveSchedules.ToList();
            }
        }
    }
}