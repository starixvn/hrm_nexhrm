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
            var result = _repository.Insert(model);

            return result;
        }

        public IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model)
        {
            var result = _repository.GetSummaryReport(model);

            return result;
        }

        public bool UpdateStatus(int id, OTRequestStatus status)
        {
            var result = _repository.UpdateStatus(id, status);

            return result;
        }

        public OverTime Get(int id)
        {
            var model = _repository.Get(id);

            return model;
        }

        public bool Update(OverTime model)
        {
            var result = _repository.Update(model);

            return result;
        }
    }
}
