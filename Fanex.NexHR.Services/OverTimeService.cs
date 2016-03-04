namespace Fanex.NexHR.Services
{
    using Criteria.Overtime;
    using Interfaces;
    using Models;
    using Models.Overtime;
    using Repositories;
    using Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

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
            IList<Overtime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = repository.GetOvertime(criteria);
            }

            return overtimes ?? new List<Overtime>();
        }

        public Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria)
        {
            Overtime overtime = null;
            if (criteria != null && criteria.IsValid())
            {
                overtime = repository.GetOvertimeById(criteria);
            }

            return overtime;
        }

        public IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria)
        {
            IList<EmployeeOvertime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = repository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria)
        {
            IList<EmployeeOvertime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = repository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public List<OvertimeSummary> GetOvertimeSummary(OvertimeSummaryCriteria criteria)
        {
            List<OvertimeSummary> listSummary = new List<OvertimeSummary>();
            if (criteria != null && criteria.IsValid())
            {
                listSummary = repository.GetOvertimeSummary(criteria).ToList();
            }

            return listSummary;
        }
    }
}
