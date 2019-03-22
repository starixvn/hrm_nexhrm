namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.ScheduledMessage;

    public class ScheduledMessageController : BaseController
    {
        private readonly IScheduledMessageService scheduledMessageService;

        public ScheduledMessageController()
        {
            scheduledMessageService = new ScheduledMessageService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var groupConversations = await scheduledMessageService.GetGroupConversationsAsync().ConfigureAwait(false);

            var viewModel = new CreateScheduledMessageViewModel
            {
                SendDate = DateTime.Today,
                GroupConversations = new SelectList(groupConversations, nameof(GroupConversation.ConversationId), nameof(GroupConversation.GroupName))
            };

            return PartialView("_Create", viewModel);
        }
    }
}