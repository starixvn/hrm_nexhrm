namespace Fanex.NexHR.Services
{
    using Criteria.OverTime;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;
    using System.Collections.Generic;

    public class OverTimeService : BaseService, IOverTimeService
    {
        private readonly IOverTimeRepository _repository;

        public OverTimeService()
            : this(null)
        {
        }

        public OverTimeService(IOverTimeRepository repository)
        {
            _repository = repository ?? new OverTimeRepository();
        }

        public CommandResult Insert(InsertOverTimeCriteria criteria)
        {
            var result = Repository.Insert(criteria);

            return result;
        }

        public IEnumerable<OverTime> GetSummaryReport(GetSummaryCriteria criteria)
        {
            var result = _repository.GetSummaryReport(criteria);

            return result;
        }

        public CommandResult ApproveRequest(ApproveOverTimeRequestCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public CommandResult DisapproveRequest(DisapproveOverTimeRequestCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public CommandResult RejectRequest(UpdateStatusCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public OverTime Get(GetByIdCriteria criteria)
        {
            var model = _repository.Get(criteria);

            return model;
        }

        public CommandResult Update(UpdateOverTimeCriteria criteria)
        {
            return Repository.Update(criteria);
        }
    }
}
