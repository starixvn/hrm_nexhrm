namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Criteria.Report;
    using Models;
    using Services;
    using Services.Interfaces;

    public class AjaxController : BaseController
    {
        private readonly ICommandService _commandService;

        public AjaxController()
            : this(null)
        {
        }

        public AjaxController(
            ICommandService commandService)
        {
            _commandService = commandService ?? new CommandService();
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

            CommandResult commandResult = _commandService.Update(criteria);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }
    }
}