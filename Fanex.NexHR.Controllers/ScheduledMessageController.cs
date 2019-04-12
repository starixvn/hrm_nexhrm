namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.ScheduledMessage;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ScheduledMessageController : BaseController
    {
        private readonly IScheduledMessageService scheduledMessageService;
        private readonly IEmployeeService employeeService;

        public ScheduledMessageController()
        {
            scheduledMessageService = new ScheduledMessageService();
            employeeService = new EmployeeService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Search(int year)
        {
            var scheduledMessages = await scheduledMessageService.GetScheduledMessagesAsync(year, CurrentUser.Id).ConfigureAwait(false);

            var viewModel = scheduledMessages.Select(x => new ScheduledMessageViewModel
            {
                ScheduledMessageId = x.ScheduledMessageId,
                ConversationId = x.ConversationId,
                ConversationName = x.ConversationName,
                Content = x.Content,
                SendDate = x.SendDate,
                CreatedBy = x.CreatedBy,
                NickName = x.NickName,
                IsSent = x.IsSent
            });

            return PartialView("_ScheduleMessages", viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var conversations = await scheduledMessageService.GetConversationsAsync(CurrentUser.Id).ConfigureAwait(false);

            var viewModel = new CreateScheduledMessageViewModel
            {
                SendDate = DateTime.Today,
                Hour = 8,
                Minute = 0,
                Conversations = new SelectList(conversations, nameof(Conversation.ConversationId), nameof(Conversation.ConversationName))
            };

            return PartialView("_Create", viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var conversations = await scheduledMessageService.GetConversationsAsync(CurrentUser.Id).ConfigureAwait(false);
            var scheduledMessage = await scheduledMessageService.GetScheduledMessageAsync(id).ConfigureAwait(false);

            var viewModel = new EditScheduledMessageViewModel
            {
                ScheduledMessageId = scheduledMessage.ScheduledMessageId,
                ConversationId = scheduledMessage.ConversationId,
                SendDate = scheduledMessage.SendDate.Date,
                Hour = scheduledMessage.SendDate.Hour,
                Minute = scheduledMessage.SendDate.Minute,
                Content = scheduledMessage.Content,
                Conversations = new SelectList(conversations, nameof(Conversation.ConversationId), nameof(Conversation.ConversationName))
            };

            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateScheduledMessageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.SendDate = viewModel.SendDate.AddHours(viewModel.Hour).AddMinutes(viewModel.Minute);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditScheduledMessageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.SendDate = viewModel.SendDate.AddHours(viewModel.Hour).AddMinutes(viewModel.Minute);

                var result = await scheduledMessageService.EditAsync(new EditScheduledMessage
                {
                    ScheduledMessageId = viewModel.ScheduledMessageId,
                    ConversationId = viewModel.ConversationId,
                    SendDate = viewModel.SendDate,
                    Content = viewModel.Content,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Remove(int id)
        {
            if (id > 0)
            {
                var result = await scheduledMessageService.RemoveAsync(id).ConfigureAwait(false);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = result.Message
                });
            }

            return Json(new { isSuccessful = false });
        }

        [HttpGet]
        public ActionResult CreateConversation()
        {
            var hasHRRole = User.IsInRole(Role.HR.EnumToString());

            var viewModel = new CreateConversationViewModel
            {
                HasHRRole = hasHRRole
            };

            if (hasHRRole)
            {
                var employees = employeeService.Query(new GetEmployeeCriteria
                {
                    Query = string.Empty,
                    DeleteStatus = false,
                    PageSize = int.MaxValue,
                    PageNumber = 1
                });

                viewModel.Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName));
            }

            return PartialView("_CreateConversation", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateConversation(CreateConversationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await scheduledMessageService.CreateConversationAsync(new CreateConversation
                {
                    ConversationName = viewModel.ConversationName,
                    ConversationId = viewModel.ConversationId,
                    CreatedBy = CurrentUser.Id,
                    PermittedEmployeeIds = viewModel.PermittedEmployeeIds != null ? string.Join(",", viewModel.PermittedEmployeeIds) : null,
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

        [HttpGet]
        public ActionResult ManageConversations()
        {
            return View("ManageConversations", User.IsInRole(Role.HR.EnumToString()));
        }

        [HttpGet]
        public async Task<ActionResult> SearchConversation()
        {
            var conversations = await scheduledMessageService.GetConversationsByCreatorAsync(CurrentUser.Id).ConfigureAwait(false);
            var viewModel = conversations.Select(x => new ConversationViewModel
            {
                Id = x.Id,
                ConversationId = x.ConversationId,
                ConversationName = x.ConversationName,
                PermittedEmployees = string.Join(", ", x.PermittedEmployees.Select(p => p.NickName))
            });

            return PartialView("_Conversations", viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> EditConversation(int id)
        {
            var conversation = await scheduledMessageService.GetConversationByIdAsync(id).ConfigureAwait(false);
            var viewModel = new EditConversationViewModel
            {
                Id = conversation.Id,
                ConversationId = conversation.ConversationId,
                ConversationName = conversation.ConversationName,
                PermittedEmployeeIds = conversation.PermittedEmployees.Select(x => x.EmployeeId).ToArray(),
                HasHRRole = User.IsInRole(Role.HR.EnumToString())
            };

            if (viewModel.HasHRRole)
            {
                var employees = employeeService.Query(new GetEmployeeCriteria
                {
                    Query = string.Empty,
                    DeleteStatus = false,
                    PageSize = int.MaxValue,
                    PageNumber = 1
                });

                viewModel.Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.NickName));
            }

            return PartialView("_EditConversation", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditConversation(EditConversationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await scheduledMessageService.EditConversationAsync(new EditConversation
                {
                    Id = viewModel.Id,
                    ConversationName = viewModel.ConversationName,
                    ConversationId = viewModel.ConversationId,
                    EditedBy = CurrentUser.Id,
                    PermittedEmployeeIds = viewModel.PermittedEmployeeIds != null ? string.Join(",", viewModel.PermittedEmployeeIds) : null,
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