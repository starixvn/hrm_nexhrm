namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
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

            IList<Memo> entityList = _memoService.GetAll(getAllMemoCriteria);

            return entityList.AsQueryable();
        }
    }
}