namespace Fanex.NexHR.Services
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class CalendarService : BaseService, ICalendarService
    {
        private readonly ICalendarRepository calendarRepository;

        public CalendarService(ICalendarRepository calendarRepository = null)
        {
            this.calendarRepository = calendarRepository ?? new CalendarRepository();
        }

        public CommandResult UpdateEmployeeShiftDate(UpdateEmployeeShiftDateCriteria criteria)
        {
            return Execute(criteria);
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingScheduleModel> workingSchedules = new List<WorkingScheduleModel>();
            if (criteria.IsValid())
            {
                workingSchedules = calendarRepository.GetWorkingSchedule(criteria);
            }

            return workingSchedules;
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarsViewModel> workingCalendars = new List<WorkingCalendarsViewModel>();
            if (criteria.IsValid())
            {
                workingCalendars = calendarRepository.GetWorkingCalendars(criteria);
            }

            return workingCalendars;
        }

        public IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<UserChangedShift> changeShifts = new List<UserChangedShift>();
            if (criteria.IsValid())
            {
                changeShifts = GetData<UserChangedShift>(criteria).ToList();
            }

            return changeShifts;
        }

        public IList<HalfDayOff> GetHalfDayOffs(GetHalfDayOffCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<HalfDayOff> halfDayOffs = new List<HalfDayOff>();
            if (criteria.IsValid())
            {
                halfDayOffs = GetData<HalfDayOff>(criteria).ToList();
            }

            return halfDayOffs;
        }

        public CommandResult InsertWorkingCalendar(InsertEmployeeCalendarCriteria criteria)
        {
            return Execute(criteria);
        }
    }
}