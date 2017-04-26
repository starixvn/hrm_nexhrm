namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
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
    }
}