namespace Fanex.NexHR.Services
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

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

        public IList<WorkingSchedule> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingSchedule> workingSchedules = new List<WorkingSchedule>();
            if (criteria.IsValid())
            {
                workingSchedules = calendarRepository.GetWorkingSchedule(criteria);
            }

            return workingSchedules;
        }

        public CommandResult InsertWorkingCalendar(InsertEmployeeCalendarCriteria criteria)
        {
            return Execute(criteria);
        }
    }
}