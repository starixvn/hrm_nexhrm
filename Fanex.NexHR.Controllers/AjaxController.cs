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

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            if (criteria.InOutType == null)
            {
                const short typeOfNone = 2;
                criteria.InOutType = typeOfNone;
                criteria.ModifiedBy = CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }
            else
            {
                criteria.ModifiedBy = CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessInOutUser(TimeRecordUpdateTypeCriteria criteria)
        {
            var commandResult = new CommandResult();
            if (criteria.EmployeeId == CurrentUser.Id)
            {
                if (criteria.InOutType == null)
                {
                    const short typeOfNone = 2;
                    criteria.InOutType = typeOfNone;
                    criteria.ModifiedBy = CurrentUser.Id;
                    criteria.ModifiedDate = DateTime.Now;
                }
                else
                {
                    criteria.ModifiedBy = CurrentUser.Id;
                    criteria.ModifiedDate = DateTime.Now;
                }

                commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = "No permission to update";
            }

            return Json(commandResult);
        }
    }
}