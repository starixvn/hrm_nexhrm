namespace Fanex.NexHR.Controllers
{
    using System;
    using Criteria.Report;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;

    public class AjaxController : BaseController
    {
        private readonly ICommandService commandService;

        public AjaxController(ISessionService sessionService, ICommandService commandService) : base(sessionService)
        {
            this.commandService = commandService;
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = commandService.ExecuteNonQuery(criteria);

            return Json(commandResult);
        }
    }
}