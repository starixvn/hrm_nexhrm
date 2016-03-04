namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
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

        public IList<WorkingSchedule> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            IList<WorkingSchedule> workingSchedules = null;
            if (criteria != null && criteria.IsValid())
            {
                workingSchedules = calendarRepository.GetWorkingSchedule(criteria);
            }

            return workingSchedules ?? new List<WorkingSchedule>();
        }
    }
}