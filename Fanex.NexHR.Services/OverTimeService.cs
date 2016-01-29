namespace Fanex.NexHR.Services
{
    using Core.Enums;
    using Criteria.OverTime;
    using Models;
    using Repositories;
    using Repositories.Interfaces;
    using Interfaces;
    using ViewModels;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class OverTimeService : IOverTimeService
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

        public bool Insert(OverTimeViewModel model)
        {
            var record = JsonConvert.DeserializeObject<IList<OverTimeRecord>>(model.RecordOTSheet);            
            var createCriteria = new InsertOverTimeCriteria
            {
                CreatedBy = model.CreatedId,
                Description = model.Description,
                Date = model.Date,
                Status = model.Status,
                OverTimeRecord = record
            };

            var result = _repository.Insert(createCriteria);

            return result;
        }

        public bool InsertAndApprove(OverTimeViewModel model)
        {
            var record = JsonConvert.DeserializeObject<IList<OverTimeRecord>>(model.RecordOTSheet);
            var createCriteria = new InsertAndApproveOverTimeCriteria
            {
                CreatedBy = model.CreatedId,
                Description = model.Description,
                Date = model.Date,
                Status = model.Status,
                OverTimeRecord = record
            };

            var result = _repository.Insert(createCriteria);

            return result;
        }


        public IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model)
        {
            var result = _repository.GetSummaryReport(model);

            return result;
        }

        public bool ApproveRequest(int id, int userId)
        {
            var result = IsValidStatusForApprove(id);

            return result ? _repository.ApproveRequest(id, userId) : result;
        }

        public bool DisapproveRequest(int id, int userId)
        {
            var result = IsValidStatusForDisapprove(id);

            return result ? _repository.DisapproveRequest(id, userId) : result;
        }

        public bool RejectRequest(int id, int userId)
        {
            var result = IsValidStatusForReject(id);

            return result ? _repository.UpdateStatus(id, RequestStatuse.Reject, userId) : result;
        }

        public OverTime Get(int id)
        {
            var model = _repository.Get(id);

            return model;
        }

        public bool Update(OverTimeViewModel viewModel)
        {
            var result = IsValidStatusForUpdate(viewModel.Id);

            return result ? _repository.Update(viewModel) : result;
        }

        private bool IsValidStatusForUpdate(int id)
        {
            var model = Get(id);

            return (model != null && model.Status == (int)RequestStatuse.Pending);
        }

        private bool IsValidStatusForDisapprove(int id)
        {
            var model = Get(id);

            return (model != null && model.Status == (int)OTRequestStatus.Approved);
        }

        private bool IsValidStatusForReject(int id)
        {
            var model = Get(id);

            return (model != null && model.Status == (int)OTRequestStatus.Pending);
        }

        private bool IsValidStatusForApprove(int id)
        {
            var model = Get(id);

            return (model != null && model.Status == (int)OTRequestStatus.Pending);
        }
    }
}
