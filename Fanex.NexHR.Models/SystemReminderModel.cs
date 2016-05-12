namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    public class SystemReminderModel
    {
        public List<Employee> EmployeeHasBirthday { get; set; }

        public List<Employee> EmployeeHasFirstWorkingDay { get; set; }

        public List<CompanyEvent.CompanyEvent> EventList { get; set; }
    }
}
