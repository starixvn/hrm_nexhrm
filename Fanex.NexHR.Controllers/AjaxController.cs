namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Criteria.Report;
    using Fanex.NexHR.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;

    public class AjaxController : BaseController
    {
        private readonly IBaseService baseService;

        public AjaxController(ISessionService sessionService, IBaseService baseService) : base(sessionService)
        {
            this.baseService = baseService;
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(commandResult);
        }
    }
}