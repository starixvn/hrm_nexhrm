namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.Overtime;
    using Fanex.Common.Helper;
    using Fanex.NexHR.ViewModels;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;

    public class OvertimeService : BaseService, IOvertimeService
    {
        private readonly IOvertimeRepository repository;

        public OvertimeService()
            : this(null)
        {
        }

        public OvertimeService(IOvertimeRepository repository)
        {
            this.repository = repository ?? new OvertimeRepository();
        }

        public IList<Overtime> GetOvertime(GetOvertimeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Overtime> overtimes = null;
            if (criteria.IsValid())
            {
                overtimes = repository.GetOvertime(criteria);
            }

            return overtimes ?? new List<Overtime>();
        }

        public Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Overtime overtime = null;
            if (criteria.IsValid())
            {
                overtime = repository.GetOvertimeById(criteria);
            }

            return overtime;
        }

        public IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<EmployeeOvertime> overtimes = null;
            if (criteria.IsValid())
            {
                overtimes = repository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<EmployeeOvertime> overtimes = null;
            if (criteria.IsValid())
            {
                overtimes = repository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }
    }
}
