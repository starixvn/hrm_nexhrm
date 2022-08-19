namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.ScheduledMessage;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ScheduledMessageController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IScheduledMessageService scheduledMessageService;
        private readonly IEmployeeService employeeService;

        public ScheduledMessageController(ISessionService sessionService,
            IBaseService baseService,
            IScheduledMessageService scheduledMessageService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.scheduledMessageService = scheduledMessageService;
            this.employeeService = employeeService;
            this.baseService = baseService;
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
                SkypeConversationId = x.SkypeConversationId,
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
            var repeatTypes = baseService.GetData<CronExpression>(new GetRepeatTypesCriteria());

            var viewModel = new CreateScheduledMessageViewModel
            {
                SendDate = DateTime.Today,
                Hour = 8,
                Minute = 0,
                StepNumber = 1,
                Conversations = new SelectList(conversations, nameof(Conversation.ConversationId), nameof(Conversation.ConversationName)),
                RepeatTypes = new SelectList(repeatTypes, nameof(CronExpression.Id), nameof(CronExpression.Description))
            };

            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateScheduledMessageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.SendDate = viewModel.SendDate.AddHours(viewModel.Hour).AddMinutes(viewModel.Minute);
                string cron = ConvertConditionToCron(viewModel.StepNumber, viewModel.RepeatTypeId,
                    viewModel.SendDate);

                var result = await scheduledMessageService.CreateAsync(new CreateScheduledMessage
                {
                    ConversationId = viewModel.ConversationId,
                    SendDate = viewModel.SendDate,
                    Content = viewModel.Content,
                    CreatedBy = CurrentUser.Id,
                    CronExpression = cron,
                    StepNumber = viewModel.StepNumber,
                    RepeatTypeId = viewModel.IsRepeat ? viewModel.RepeatTypeId : null
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

        private static string ConvertConditionToCron(int stepNumber, int? repeatTypeId, DateTime startDate)
        {
            switch (repeatTypeId)
            {
                case Constants.RepeatMinute:
                    return $"*/{stepNumber} * * * *";

                case Constants.RepeatHour:
                    return $"{startDate.Minute} */{stepNumber} * * *";

                case Constants.RepeatDay:
                    return $"{startDate.Minute} {startDate.Hour} */{stepNumber} * *";

                case Constants.RepeatWeek:
                    return $"{startDate.Minute} {startDate.Hour} */{stepNumber * Constants.NumberOfDayInWeek} * *";

                case Constants.RepeatMonth:
                    return $"{startDate.Minute} {startDate.Hour} {startDate.Day} */{stepNumber} *";

                case Constants.RepeatYear:
                    return $"{startDate.Minute} {startDate.Hour} {startDate.Day} {startDate.Month} *";

                default:
                    return null;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var conversations = await scheduledMessageService.GetConversationsAsync(CurrentUser.Id).ConfigureAwait(false);
            var scheduledMessage = await scheduledMessageService.GetScheduledMessageAsync(id).ConfigureAwait(false);
            var repeatTypes = baseService.GetData<CronExpression>(new GetRepeatTypesCriteria());

            var viewModel = new EditScheduledMessageViewModel
            {
                ScheduledMessageId = scheduledMessage.ScheduledMessageId,
                ConversationId = scheduledMessage.ConversationId,
                SendDate = scheduledMessage.SendDate.Date,
                Hour = scheduledMessage.SendDate.Hour,
                Minute = scheduledMessage.SendDate.Minute,
                Content = scheduledMessage.Content,
                RepeatTypeId = scheduledMessage.RepeatTypeId,
                StepNumber = scheduledMessage.StepNumber == 0 ? 1 : scheduledMessage.StepNumber,
                IsRepeat = scheduledMessage.StepNumber != 0,
                Conversations = new SelectList(conversations, nameof(Conversation.ConversationId), nameof(Conversation.ConversationName)),
                RepeatTypes = new SelectList(repeatTypes, nameof(CronExpression.Id), nameof(CronExpression.Description))
            };

            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditScheduledMessageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.SendDate = viewModel.SendDate.AddHours(viewModel.Hour).AddMinutes(viewModel.Minute);
                string cron = ConvertConditionToCron(viewModel.StepNumber, viewModel.RepeatTypeId,
                    viewModel.SendDate);

                var result = await scheduledMessageService.EditAsync(new EditScheduledMessage
                {
                    ScheduledMessageId = viewModel.ScheduledMessageId,
                    ConversationId = viewModel.ConversationId,
                    SendDate = viewModel.SendDate,
                    Content = viewModel.Content,
                    CronExpression = cron,
                    StepNumber = viewModel.StepNumber,
                    RepeatTypeId = viewModel.IsRepeat ? viewModel.RepeatTypeId : null
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
            var userRoles = employeeService.UserRoles(CurrentUser);
            var hasHRRole = userRoles.Any(item => item == "HR");

            var viewModel = new CreateConversationViewModel
            {
                HasHRRole = hasHRRole
            };

            if (hasHRRole)
            {
                var employees = employeeService.Query(new GetEmployeeCriteria
                {
                    Query = string.Empty,
                    IsDelete = false,
                    IsDisable = false,
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
                    SkypeConversationId = viewModel.SkypeConversationId,
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
            var userRoles = employeeService.UserRoles(CurrentUser);
            ViewBag.HasHRRole = userRoles.Any(item => item == "HR");
            return View("ManageConversations");
        }

        [HttpGet]
        public async Task<ActionResult> SearchConversation()
        {
            var conversations = await scheduledMessageService.GetConversationsByCreatorAsync(CurrentUser.Id).ConfigureAwait(false);
            var viewModel = conversations.Select(x => new ConversationViewModel
            {
                ConversationId = x.ConversationId,
                SkypeConversationId = x.SkypeConversationId,
                ConversationName = x.ConversationName,
                IsActive = x.IsActive,
                PermittedEmployees = string.Join(", ", x.PermittedEmployees.Select(p => p.NickName))
            });
            var userRoles = employeeService.UserRoles(CurrentUser);
            ViewBag.HasHRRole = userRoles.Any(item => item == "HR");

            return PartialView("_Conversations", viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> EditConversation(int id)
        {
            var userRoles = employeeService.UserRoles(CurrentUser);
            var conversation = await scheduledMessageService.GetConversationByIdAsync(id).ConfigureAwait(false);
            var viewModel = new EditConversationViewModel
            {
                ConversationId = conversation.ConversationId,
                SkypeConversationId = conversation.SkypeConversationId,
                ConversationName = conversation.ConversationName,
                PermittedEmployeeIds = conversation.PermittedEmployees.Select(x => x.EmployeeId).ToArray(),
                IsActive = conversation.IsActive,
                HasHRRole = userRoles.Any(item => item == "HR")
            };

            if (viewModel.HasHRRole)
            {
                var employees = employeeService.Query(new GetEmployeeCriteria
                {
                    Query = string.Empty,
                    IsDelete = false,
                    IsDisable = false,
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
                    ConversationId = viewModel.ConversationId,
                    ConversationName = viewModel.ConversationName,
                    SkypeConversationId = viewModel.SkypeConversationId,
                    EditedBy = CurrentUser.Id,
                    IsActive = viewModel.IsActive,
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