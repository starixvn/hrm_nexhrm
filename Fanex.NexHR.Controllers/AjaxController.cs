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
        private readonly IRequestService requestService;
        private readonly IExtensionNoService extensionNoService;
        private readonly ICommandService commandService;

        public AjaxController()
            : this(null, null, null)
        {
        }

        public AjaxController(
            IRequestService requestService,
            IExtensionNoService extensionNoService,
            ICommandService commandService)
        {
            this.requestService = requestService ?? new RequestService();
            this.extensionNoService = extensionNoService ?? new ExtensionNoService();
            this.commandService = commandService ?? new CommandService();
        }

        [Authorize(Roles = "AssistantLeader,HR")]
        [HttpPost]
        public JsonResult ProcessRequest(string command, long id)
        {
            CommandResult result = requestService.Process(command, id, this.CurrentUser.Id);

            if (result.Code == 0)
            {
                return Json(true);
            }

            return Json(result.Message);
        }

        [HttpPost]
        public string GetRequestDetail(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        {
            request.RequestType = new RequestTypeModel { Id = requestType };
            request.FromShift = new Shift { Name = fromShiftCode };
            request.ToShift = new Shift { Name = toShiftCode };

            string requestDetail = RequestService.ToRequestMessage(request);
            return requestDetail;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExtensionNumber(int extId)
        {
            var criteria = new DeleteExtensionNumberCriteria
            {
                ExtId = extId
            };
            CommandResult result = commandService.Delete(criteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool AddNewExternalGroup(string groupName)
        {
            var criteria = new AddNewExternalGroupCriteria
            {
                FullName = groupName
            };
            CommandResult result = commandService.Insert(criteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExternalGroup(int extId)
        {
            var criteria = new DeleteExternalGroupCriteria
            {
                ExtId = extId
            };
            CommandResult result = commandService.Delete(criteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool EditExternalGroup(int extId, string groupName)
        {
            var criteria = new UpdateExternalGroupCriteria
            {
                ExtId = extId,
                FullName = groupName
            };
            CommandResult result = commandService.Update(criteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = commandService.Update(criteria);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }
    }
}