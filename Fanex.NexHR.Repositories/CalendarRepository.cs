namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Repositories.Interfaces;

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
    }
}