namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Fanex.NexHR.Core.Helpers;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Conversation;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Models.LogTime;
    using Fanex.NexHR.Models.Staff;
    using Fanex.NexHR.Models.TimeSheet;
    using Fanex.NexHR.Resources;
    using Interfaces;
    using Models.NexBot;
    using Models.Request;
    using Newtonsoft.Json;
    using NPOI.SS.Formula.Functions;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public class NexbotService : INexbotService
    {
        private readonly HttpClient httpClient;
        private readonly IBaseService baseService;
        private readonly ChatbotConfigs nexBotSettings;
        private readonly ILogServiceProvider logService;
        private readonly AppSetting appSetting;
        private const string conversationIdName = "conversationId";

        public NexbotService(IHttpClientFactory httpClientFactory,
            IBaseService baseService,
            AppSetting appSetting,
            ChatbotConfigs nexBotSettings,
            ILogServiceProvider logService)
        {
            httpClient = httpClientFactory.CreateClient();
            this.nexBotSettings = nexBotSettings;
            this.baseService = baseService;
            this.appSetting = appSetting;
            this.logService = logService;
        }

        public async Task<bool> SendMessageAsync(string conversationId, string message)
        {
            if (nexBotSettings.Enable && !string.IsNullOrWhiteSpace(conversationId))
            {
                var request = CreateRequestAsync("/messages/forward", HttpMethod.Post);
                var data = new Dictionary<string, string>
                {
                    { conversationIdName, conversationId },
                    { "message", message }
                };

                request.Content = new FormUrlEncodedContent(data);

                using var response = await httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }

            return false;
        }

        public async Task SendRemindCheckInCheckOutAsync(MissingCheckInOutEmployee missingCheckInOutEmployee)
        {
            if (nexBotSettings.Enable && !string.IsNullOrWhiteSpace(missingCheckInOutEmployee.ConversationId))
            {
                var request = CreateRequestAsync("/remind/MissingCheckInCheckOut", HttpMethod.Post);
                var data = new Dictionary<string, string>
                {
                    { conversationIdName, missingCheckInOutEmployee.ConversationId },
                    { "empId", missingCheckInOutEmployee.EmpId.ToString()},
                    { "nickName", missingCheckInOutEmployee.NickName },
                    { "shift", missingCheckInOutEmployee.ShiftCode },
                    { "attendanceDate", missingCheckInOutEmployee.AttendanceDate.ToString(CultureInfo.InvariantCulture) },
                    { "missingType", missingCheckInOutEmployee.Type.ToString() }
                };

                request.Content = new FormUrlEncodedContent(data);

                using var response = await httpClient.SendAsync(request);
            }
        }

        private HttpRequestMessage CreateRequestAsync(string endPointPath, HttpMethod method)
        {
            Uri url = new($"{nexBotSettings.BotServiceUrl}{endPointPath}");
            var request = new HttpRequestMessage(method, url);
            return request;
        }

        public async Task<IList<ConversationUser>> GetSkypeAccountAsync()
         => await SendHttpClientAsync<IList<ConversationUser>>("/account/index", HttpMethod.Get);

        public async Task<IList<ConversationUser>> GetIgnoredSkypeAccountByMessageCategoryAsync(MessageCategory messageCategory)
            => await SendHttpClientAsync<IList<ConversationUser>>(
                $"/account/GetIgnoredAccountByMessageCategory?messageCategoryId={(int)messageCategory}",
                HttpMethod.Get);

        public async Task<IList<ConversationUser>> GetRegisteredConversationAsync(MessageCategory messageCategory)
         => await SendHttpClientAsync<IList<ConversationUser>>(
                $"/account/GetRegisteredConversationByMessageCategory?messageCategoryId={(int)messageCategory}",
                HttpMethod.Get);

        private async Task SendApproveMessageAsync(RequestMessage requestMessage)
        {
            if (nexBotSettings.Enable && !string.IsNullOrEmpty(requestMessage.ConversationId))
            {
                var jsonData = JsonConvert.SerializeObject(requestMessage);
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                await SendHttpClientAsync<Task>("/request/process", HttpMethod.Post, data);
            }
        }

        public async Task SendRegistVegetarianMealAsync(string conversationId, int empId, int companyId, DateTime onDate, string message, string messageContent)
        {
            if (nexBotSettings.Enable)
            {
                var data = new Dictionary<string, string>
                {
                    {conversationIdName, conversationId },
                    {"empId", empId.ToString() },
                    {"companyId", companyId.ToString() },
                    {"onDate", onDate.ToString("yyyy-MM-dd") },
                    {"message", message },
                    {"messageContent", messageContent },
                    {"callBackUrl", $"{appSetting.FBUrl}/api" },
                };

                await SendHttpClientAsync<Task>("/messages/registvegetarianmeal", HttpMethod.Post, data);
            }
        }

        public async Task SendMessageForLeaveRequest(LeaveRequestViewModel request, IEnumerable<EmployeeSupervisor> employeeSupervisors)
        {
            if (employeeSupervisors != null && employeeSupervisors.Any())
            {
                foreach (var employeeSupervisor in employeeSupervisors)
                {
                    var message = GenerateMessageForLeaveRequest(request, employeeSupervisor.SupervisorNickName);
                    var forDate = GetDateStringForLeaveRequest(request.FromDate, request.ToDate, request.TotalDays);
                    var requestMessage = new RequestMessage
                    {
                        RequestId = request.Id,
                        ConversationId = employeeSupervisor.SupervisorConversationId,
                        ProcessedBy = employeeSupervisor.SupervisorId,
                        Message = message,
                        Supervisor = employeeSupervisor.SupervisorNickName,
                        Nick = employeeSupervisor.NickName,
                        FullName = employeeSupervisor.FullName,
                        Code = employeeSupervisor.Code,
                        CreatedDate = (request.IsEdit ? "Edited at " : "Created at ") + DateTimeOffset.Now.DateTime.ToClientTimeZone().ToString("dd MMM, yyyy HH:mm"),
                        RequestDate = forDate,
                        RequestType = "leave",
                        LeaveType = request.LeaveTypeName,
                        Remark = request.Remark,
                        IsReadOnly = employeeSupervisor.IsReadOnly,
                        CallBackUrl = $"{appSetting.BaseUrl}/api",
                        AvatarUrl = await FileHelpers.GetAvatar(employeeSupervisor.Code)
                    };

                    await SendApproveMessageAsync(requestMessage);
                }
            }
        }

        private string GenerateMessageForLeaveRequest(LeaveRequestViewModel request, string leaderName)
        {
            var forDate = GetDateStringForLeaveRequest(request.FromDate, request.ToDate, request.TotalDays);
            var message = $"Dear {leaderName}, {Constants.NewLine}";
            message += $"This is inform you about your member, {request.Employee} had a leave request (#{request.Id}): {Constants.NewLine}";
            message += $"_- Date: {forDate}_ {Constants.NewLine}";
            message += $"_- Type: {request.LeaveTypeName}_ {Constants.NewLine}";
            message += $"_- Remark: {request.Remark}_ {Constants.NewLine}";
            message += $"Please process this request at: {appSetting.BaseUrl}/request/member.  {Constants.NewLine}";
            message += "Any questions, please contact HR department.";
            return message;
        }

        private static string GetDateStringForLeaveRequest(DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            if (totalDays > 1)
            {
                return string.Format(
                    Constants.MailContentLeave,
                    fromDate.ToDateMonthYearFormatedDate(),
                    toDate.ToDateMonthYearFormatedDate(),
                    totalDays);
            }
            else
            {
                if (totalDays == 1)
                {
                    return string.Format(Constants.MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
                }

                return string.Format(
                        Constants.MailContentLeaveHalfDay,
                        fromDate.ToDateMonthYearFormatedDate(),
                        fromDate.ToString("HH:mm"),
                        toDate.ToString("HH:mm"));
            }
        }

        public async Task SendMessageProcessRequest(RequestEntity request, string command)
        {
            var criteria = new GetEmployeeConversationByIdCriteria
            {
                EmpId = request.Requester.Id
            };
            var conversation = await baseService.GetDataEntityAsync<Employee>(criteria);
            if (conversation != null && !string.IsNullOrWhiteSpace(conversation.ConversationId))
            {
                var message = GenerateProcessRequestMessage(request, command);
                await SendMessageAsync(conversation.ConversationId, message);
            }
        }

        public async Task<CommandResult> SendMessageDeductToConversation(IEnumerable<string> conversations, string message)
        {
            var result = false;
            foreach (var conversation in conversations)
            {
                result = await SendMessageAsync(conversation, message).ConfigureAwait(false);
            }

            return new CommandResult { Code = result ? 0 : 1 };
        }

        public async Task<CommandResult> SendMessageReminderLateInEarlyOutMoreThanSixtyMinuteToEmp(IEnumerable<ReminderLateInEarlyOut> reminderLateInEarlyOuts)
        {
            var result = false;
            foreach (var conversation in reminderLateInEarlyOuts)
            {
                if (!string.IsNullOrWhiteSpace(conversation.ConversationId))
                {
                    var message = string.Format(Messages.MessageReminderLateInEarlyOut, conversation.NickName,
                    conversation.AttendanceDate.ToDateMonthYearFormatedDate(), appSetting.BaseUrl);
                    result = await SendMessageAsync(conversation.ConversationId, message).ConfigureAwait(false);
                }
            }

            return new CommandResult { Code = result ? 0 : 1 };
        }

        private string GenerateProcessRequestMessage(RequestEntity request, string command)
        {
            var action = command == Command.Reject.EnumToString() ? $"{command.ToLowerInvariant()}ed" : $"{command.ToLowerInvariant()}d";

            var forDate = request.TotalDays > 1
                ? "from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " +
                  request.ToDate.ToDateMonthYearFormatedDate()
                : "on " + request.FromDate.ToDateMonthYearFormatedDate();
            var message = $"Dear {request.Requester.NickName},{Constants.NewLine}";
            message += $"Your {request.RequestType.Name.ToLowerInvariant()} request _{forDate}_ has been {action} by _{request.ModifiedBy.DisplayName}._{Constants.NewLine}";
            message += $"For more detail, please visit: {appSetting.BaseUrl}. Thanks";
            return message;
        }

        public async Task SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request, IEnumerable<EmployeeSupervisor> employeeSupervisors)
        {
            try
            {
                string requestType = request.CheckInOutType == (short)RequestType.CheckIn
                            ? RequestType.CheckIn.EnumToString().ToLower(CultureInfo.InvariantCulture)
                            : RequestType.CheckOut.EnumToString().ToLower(CultureInfo.InvariantCulture);

                foreach (var employeeSupervisor in employeeSupervisors)
                {
                    var message = GenerateCheckInOutRequestMessage(request, employeeSupervisor.SupervisorNickName);
                    var requestMessage = new RequestMessage
                    {
                        RequestId = request.Id,
                        ConversationId = employeeSupervisor.SupervisorConversationId,
                        ProcessedBy = employeeSupervisor.SupervisorId,
                        Message = message,
                        Supervisor = employeeSupervisor.SupervisorNickName,
                        Nick = employeeSupervisor.NickName,
                        FullName = employeeSupervisor.FullName,
                        Code = employeeSupervisor.Code,
                        CreatedDate = request.IsEdit ? "Edited at " : "Created at " + DateTimeOffset.Now.DateTime.ToClientTimeZone().ToString("dd MMM, yyyy HH:mm"),
                        RequestDate = request.WorkDate.ToDateMonthYearFormatedDate(),
                        Shift = request.Shift,
                        Time = request.Hour.ToString().PadLeft(2, '0') + ":" + request.Minute.ToString().PadLeft(2, '0'),
                        RequestType = requestType,
                        CallBackUrl = $"{appSetting.BaseUrl}/api",
                        AvatarUrl = await FileHelpers.GetAvatar(employeeSupervisor.Code)
                    };

                    await SendApproveMessageAsync(requestMessage);
                }
            }
            catch (Exception ex)
            {
                logService.LogException(ex);
            }
        }

        public async Task<CommandResult> SendMessageDeductLateInEarlyOutToEmp(LateInEarlyOutModel lateInEarlyOutModel)
        {
            var result = new CommandResult();
            try
            {
                var criteria = new GetEmployeeConversationByIdCriteria { EmpId = lateInEarlyOutModel.EmpId };
                var conversations = await baseService.GetDataAsync<Employee>(criteria);
                if (conversations.Any())
                {
                    string message = string.Format(Messages.MessageAutoDeductLateInEarlyOut, lateInEarlyOutModel.NickName);
                    result = await SendMessageDeductToConversation(conversations.Select(m => m.ConversationId), message);
                }
            }
            catch (Exception ex)
            {
                logService.LogException(ex);
            }
            return result;
        }

        public async Task<CommandResult> SendMessageDeductLateInEarlyOutToHrTeam(IEnumerable<LateInEarlyOutModel> lateInEarlyOutModels)
        {
            var result = false;
            try
            {
                var criteria = new GetTeamHrConversationCriteria();
                var conversations = await baseService.GetDataAsync<Employee>(criteria);
                var users = string.Join(", ", lateInEarlyOutModels.Select(x => x.NickName));
                foreach (var item in conversations)
                {
                    var message = string.Format(Messages.MessageHRAutoDeductLateInEarlyOut, item.NickName, users);
                    result = await SendMessageAsync(item.ConversationId, message).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logService.LogException(ex);
            }
            return new CommandResult { Code = result ? 0 : 1 };
        }

        private string GenerateCheckInOutRequestMessage(CheckInOutRequestViewModel request, string leaderName)
        {
            string type = request.CheckInOutType == (short)RequestType.CheckIn
                            ? RequestType.CheckIn.EnumToString().ToLower(CultureInfo.InvariantCulture)
                            : RequestType.CheckOut.EnumToString().ToLower(CultureInfo.InvariantCulture);
            string time = request.Hour.ToString().PadLeft(2, '0') + ":" + request.Minute.ToString().PadLeft(2, '0');
            string message = $"Dear {leaderName}, {Constants.NewLine}";
            message += $"This is to inform you about your member, {request.Employee} had a {type} request (#{request.Id}):{Constants.NewLine}";
            message += $"_- Date: {request.WorkDate.ToDateMonthYearFormatedDate()}_{Constants.NewLine}";
            message += $"_- Shift: {request.Shift}_{Constants.NewLine}";
            message += $"_- Time: {time}_{Constants.NewLine}";
            message += $"Please process this request at: {appSetting.BaseUrl}/request/member. {Constants.NewLine}";
            message += $"Any questions, please contact HR department. Thanks";
            return message;
        }

        private async Task<T> SendHttpClientAsync<T>(string endPoint, HttpMethod method, IDictionary<string, string> data = null)
        {
            ValidateEndPoint(endPoint);

            using var request = CreateRequestAsync(endPoint, method);
            if (data is not null)
            {
                request.Content = new FormUrlEncodedContent(data);
            }

            using var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<T>(jsonResult);

                return result;
            }

            return default;
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }

        public async Task SendCheckoutNotification(CheckInCheckOutNotifyRecord checkoutRecord)
        {
            if (nexBotSettings.Enable)
            {
                var data = new Dictionary<string, string>
                {
                    {conversationIdName, checkoutRecord.ConversationId },
                    {"nickName", checkoutRecord.Nickname},
                    {"checkoutTime", checkoutRecord.CheckOutTime.ToString("hh:mm tt")},
                    {"totalHours", Utilities.TotalHoursToTime(checkoutRecord.TotalWorkHours)},
                    {"messageContent", checkoutRecord.MessageContent},
                };

                await SendHttpClientAsync<Task>("/messages/CheckoutNotification", HttpMethod.Post, data);
            }
        }

        public async Task SendCheckinNotification(CheckInCheckOutNotifyRecord checkoutRecord)
        {
            if (nexBotSettings.Enable)
            {
                var data = new Dictionary<string, string>
                {
                    {conversationIdName, checkoutRecord.ConversationId },
                    {"nickName", checkoutRecord.Nickname},
                    {"checkinTime", checkoutRecord.CheckInTime.Value.ToString("hh:mm tt")},
                    {"messageContent", checkoutRecord.MessageContent},
                };

                await SendHttpClientAsync<Task>("/messages/CheckinNotification", HttpMethod.Post, data);
            }
        }

        public async Task SendReachedWorkingHoursNotification(WorkingHour workingHour)
        {
            if (nexBotSettings.Enable)
            {
                var data = new Dictionary<string, string>
                {
                    {conversationIdName, workingHour.SkypeConversationId },
                    {"nickname", workingHour.NickName},
                    {"workingHours", Utilities.TotalHoursToTime(workingHour.TotalWorkHours)},
                    {"messageContent", workingHour.MessageContent},
                };

                await SendHttpClientAsync<Task>("/messages/ReachedWorkingHoursNotification", HttpMethod.Post, data);
            }
        }

        public async Task SendMultipleRequestsToApprover(ApproverRequestSumup approver)
        {
            if (nexBotSettings.Enable)
            {
                var data = new Dictionary<string, string>
                {
                    {"approverId", approver.ApproverId.ToString() },
                    {"approverNickname", approver.ApproverNickname},
                    {conversationIdName, approver.ConversationId},
                    {"totalRequest", approver.TotalRequest.ToString()},
                    {"requestIds", approver.RequestIds },
                    {"callbackUrl", approver.CallbackUrl},
                    {"stringRequests", JsonConvert.SerializeObject(approver.Requests.ToArray())},
                };

                await SendHttpClientAsync<Task>("/messages/RemindApproveMultipleRequest", HttpMethod.Post, data);
            }
        }

        public async Task SendMissedLogTimeRemindLeader(MissedLogTimeLeaderModel model)
        {
            if (nexBotSettings.Enable)
            {
                var data = new Dictionary<string, string>
                {
                    {conversationIdName, model.ConversationId},
                    {"leaderName", model.LeaderName},
                    {"logMonth", model.LogMonth},
                    {"StringMembers", JsonConvert.SerializeObject(model.Members.ToArray())},
                };

                await SendHttpClientAsync<Task>("/remind/MissedLogTimeLeader", HttpMethod.Post, data);
            }
        }

        public async Task SendMissedLogTimeRemindMember(MissedLogTimeMemberModel model)
        {
            if (nexBotSettings.Enable)
            {
                var data = new Dictionary<string, string>
                {
                    {conversationIdName, model.ConversationId},
                    {"nickName", model.NickName},
                    {"missedCount", model.MissedCount.ToString()},
                    {"logDate", model.LogDate},
                    {"logMonth", model.LogMonth},
                };

                await SendHttpClientAsync<Task>("/remind/MissedLogTimeMember", HttpMethod.Post, data);
            }
        }
    }
}