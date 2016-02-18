namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;
    using System.Collections.Generic;

    public interface ICalendarService
    {
        CommandResult UpdateEmployeeShiftDate(UpdateEmployeeShiftDateCriteria criteria);

        IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarCriteria criteria);

        IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria);

        IList<HalfDayOff> GetHalfDayOffs(GetHalfDayOffCriteria criteria);

        CommandResult InsertWorkingCalendar(InsertEmployeeCalendarCriteria criteria);
    }
}