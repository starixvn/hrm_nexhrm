namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Calendar;

    public class CalendarRepository : ICalendarRepository
    {
        public IList<WorkingSchedule> GetWorkingSchedule(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var workingSchedules = db.Query<WorkingSchedule, Employee, Team, WorkingSchedule>(
                     (schedule, employee, team) =>
                     {
                         employee.Team = team;
                         schedule.Employee = employee;
                         return schedule;
                     },
                     "EmpId, GroupId",
                     criteria.GetSpParams());

                return workingSchedules.ToList();
            }
        }

        public IList<LeaveSchedule> GetLeaveSchedule(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var leaveSchedules = db.Query<LeaveSchedule, Employee, Team, LeaveSchedule>(
                     (schedule, employee, team) =>
                     {
                         employee.Team = team;
                         schedule.Employee = employee;
                         return schedule;
                     },
                     "EmpId, GroupId",
                     criteria.GetSpParams());

                return leaveSchedules.ToList();
            }
        }
    }
}