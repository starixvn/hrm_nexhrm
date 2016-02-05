namespace Fanex.NexHR.Services
{
    using Criteria.Overtime;
    using Fanex.NexHR.ViewModels;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;
    using System.Collections.Generic;

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

        public CommandResult Insert(InsertOvertimeCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public IEnumerable<Overtime> ReportSummary(ReportSummaryCriteria criteria)
        {
            IEnumerable<Overtime> result = _repository.ReportSummary(criteria);

            return result;
        }

        public IEnumerable<Overtime> ReportDetail(ReportDetailCriteria criteria)
        {
            IEnumerable<Overtime> result = _repository.ReportDetail(criteria);

            return result;
        }

        public CommandResult ApproveRequest(ApproveOvertimeRequestCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public CommandResult DisapproveRequest(DisapproveOvertimeRequestCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public CommandResult RejectRequest(RejectRequestCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public OvertimeCreateViewModel Get(GetByIdCriteria criteria)
        {
            var model = _repository.Get(criteria);

            return model;
        }

        public CommandResult Update(UpdateOvertimeCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }
    }
}
