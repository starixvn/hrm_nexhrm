using Fanex.NexHR.Repositories.Interfaces;
using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fanex.NexHR.Repositories;
using Fanex.NexHR.Models;
using Fanex.NexHR.Core.Enums;
using Fanex.NexHR.Criteria.OverTime;
using Newtonsoft.Json;

namespace Fanex.NexHR.Services
{
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
            var createCriteria = new InsertOverTimeCriteria()
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

        public bool ApproveRequest(int id, int approveUserId)
        {
            var result = IsValidStatusForApprove(id);

            return result ? _repository.ApproveRequest(id, approveUserId) : result;
        }

        public bool VerifyRequest(int id, int verifyUserId)
        {
            var result = IsValidStatusForVerify(id);

            return result ? _repository.VerifyRequest(id, verifyUserId) : result;
            
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

            return (model != null && model.Status != (int)OTRequestStatus.Verified) ? true : false;
        }

        private bool IsValidStatusForVerify(int id) 
        {            
            var model = Get(id);

            return (model != null && model.Status == (int)OTRequestStatus.Approved) ? true : false;
        }

        private bool IsValidStatusForApprove(int id)
        {
            var model = Get(id);

            return (model != null && model.Status == (int)OTRequestStatus.Pending) ? true : false;
        }
    }
}
