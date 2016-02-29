namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    public class AjaxController : BaseController
    {
        private readonly ICommandService commandService;

        public AjaxController()
            : this(null)
        {
        }

        public AjaxController(
            ICommandService commandService)
        {
            this.commandService = commandService ?? new CommandService();
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = commandService.Update(criteria);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }
    }
}