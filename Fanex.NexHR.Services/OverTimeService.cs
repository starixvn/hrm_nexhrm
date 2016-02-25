namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.Overtime;
    using Fanex.NexHR.ViewModels;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;

    public class OvertimeService : BaseService, IOvertimeService
    {
        private readonly IOvertimeRepository _repository;

        public OvertimeService()
            : this(null)
        {
        }

        public OvertimeService(IOvertimeRepository repository)
        {
            _repository = repository ?? new OvertimeRepository();
        }

        public IEnumerable<Overtime> ReportSummary(ReportSummaryCriteria criteria)
        {
            var result = Enumerable.Empty<Overtime>();
            if (criteria.IsValid())
            {
                result = _repository.ReportSummary(criteria);
            }

            return result;
        }

        public IEnumerable<Overtime> ReportDetail(ReportDetailCriteria criteria)
        {
            var result = Enumerable.Empty<Overtime>();
            if (criteria.IsValid())
            {
                result = _repository.ReportDetail(criteria);
            }

            return result;
        }

        public IEnumerable<Overtime> GetMyReport(GetMyReportCriteria criteria)
        {
            var result = Enumerable.Empty<Overtime>();
            if (criteria.IsValid())
            {
                result = _repository.GetMyReport(criteria);
            }

            return result;
        }

        public Overtime Get(GetByIdCriteria criteria)
        {
            var model = _repository.Get(criteria);

            return model;
        }
    }
}
