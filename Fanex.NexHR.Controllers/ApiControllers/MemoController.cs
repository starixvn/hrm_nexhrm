namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System.Linq;
    using System.Web.Http;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    public class MemoController : ApiController
    {
        private readonly IMemoService _memoService;

        public MemoController()
            : this(null)
        {
        }

        public MemoController(IMemoService memoService)
        {
            _memoService = memoService ?? new MemoService();
        }

        public IQueryable<Memo> GetMemos(int category, int year)
        {
            var getAllMemoCriteria = new GetAllMemoCriteria()
            {
                Category = category,
                Year = year
            };
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            var entityList = _memoService.GetAll(getAllMemoCriteria);

            return entityList.AsQueryable();
        }
    }
}