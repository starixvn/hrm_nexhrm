namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.ViewModels;

    public interface ICalendarService
    {
        CommandResult UpdateEmployeeShiftDate(UpdateEmployeeShiftDateCriteria criteria);

        IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarCriteria criteria);

        IList<LeaveHalfDay> GetLeaveHalfDay(GetLeaveHalfDayCriteria criteria);

        CommandResult InsertWorkingCalendar(InsertEmployeeCalendarCriteria criteria);
    }
}