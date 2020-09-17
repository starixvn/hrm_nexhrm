namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.YearlyNexRequest;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Repositories;
    using Repositories.Interfaces;

    public class YearlyNexRequestService : BaseService, IYearlyNexRequestService
    {
        private readonly IYearlyNexRequestRepository yearlyNexRequestRepository;

        public YearlyNexRequestService()
            : this(null) { }

        public YearlyNexRequestService(IYearlyNexRequestRepository yearlyNexRequestRepository)
        {
            this.yearlyNexRequestRepository = yearlyNexRequestRepository ?? new YearlyNexRequestRepository();
        }

        public IList<YearlyNexRequest> GetYearlyNexRequest(GetYearlyNexRequestCriteria criteria)
        {
            List<YearlyNexRequest> listYearlyNexRequest = new List<YearlyNexRequest>();
            if (criteria != null && criteria.IsValid())
            {
                listYearlyNexRequest = yearlyNexRequestRepository.GetYearlyNexRequest(criteria).ToList();
            }

            return listYearlyNexRequest;
        }
    }
}