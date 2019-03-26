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
        public async Task<ActionResult> Search(int year)
        {
            var scheduledMessages = await scheduledMessageService.GetScheduledMessageAsync(year).ConfigureAwait(false);

            var viewModel = scheduledMessages.Select(x => new ScheduledMessageViewModel
            {
                ScheduledMessageId = x.ScheduledMessageId,
                ConversationId = x.ConversationId,
                GroupName = x.GroupName,
                Content = x.Content,
                SendDate = x.SendDate,
                CreatedBy = x.CreatedBy,
                NickName = x.NickName
            });

            return PartialView("_ScheduleMessages", viewModel);
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

        [HttpPost]
        public async Task<ActionResult> Create(CreateScheduledMessageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var times = viewModel.Time.Split(':');

                viewModel.SendDate = viewModel.SendDate.AddHours(int.Parse(times[0]));
                viewModel.SendDate = viewModel.SendDate.AddMinutes(int.Parse(times[1]));

                var result = await scheduledMessageService.CreateAsync(new CreateScheduledMessage
                {
                    ConversationId = viewModel.ConversationId,
                    SendDate = viewModel.SendDate,
                    Content = viewModel.Content,
                    CreatedBy = CurrentUser.Id
                }).ConfigureAwait(false);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = result.Message
                });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }
    }
}