namespace Fanex.NexHR.Services
{
    using Core;
    using Core.Enums;
    using Criteria.SkypeAccount;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Resources;
    using Interfaces;
    using Models.NexBot;
    using Models.Request;
    using Models.SkypeAccount;
    using Newtonsoft.Json;
    using Repositories.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public class NexbotService : INexbotService
    {
        private readonly HttpClient httpClient;
        private readonly INexbotRepository nexbotRepository;
        private readonly IBaseService baseService;
        private readonly NexBotSettings nexBotSettings;
        private readonly IScheduledMessageRepository scheduledMessageRepository;
        private readonly ILogServiceProvider logService;
        private readonly string BaseUrl;

        public NexbotService(IHttpClientFactory httpClientFactory,
            INexbotRepository nexbotRepository,
            IBaseService baseService,
            IScheduledMessageRepository scheduledMessageRepository,
            AppSetting appSetting,
            NexBotSettings nexBotSettings,
            ILogServiceProvider logService)
        {
            httpClient = httpClientFactory.CreateClient();
            this.nexbotRepository = nexbotRepository;
            this.nexBotSettings = nexBotSettings;
            this.scheduledMessageRepository = scheduledMessageRepository;
            this.baseService = baseService;
            BaseUrl = appSetting.BaseUrl;
            this.logService = logService;
        }

        public async Task<bool> SendMessageAsync(string conversationId, string message)
        {
            if (!string.IsNullOrWhiteSpace(conversationId))
            {
                var result = await SendMessageToBotAsync(conversationId, message);
                return result;
            }

            return false;
        }

        private async Task<HttpRequestMessage> CreateRequestAsync(string endPointPath, HttpMethod method)
        {
            var token = await GetTokenAsync();
            Uri url = new($"{nexBotSettings.BotServiceUrl}{endPointPath}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }

        private async Task<string> GetTokenAsync()
        {
            Uri url = new(nexBotSettings.TokenUrl);
            var authData = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"  },
                {"client_id", nexBotSettings.ClientId  },
                {"client_secret", nexBotSettings.ClientPassword  },
                {"scope", $"{nexBotSettings.ClientId}/.default"  },
            };

            using var authDataContent = new FormUrlEncodedContent(authData);
            using var response = await httpClient.PostAsync(url, authDataContent);
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(responseData);
            return token?.AccessToken;
        }

        public async Task<bool> SendMessageToBotAsync(string conversationId, string message)
        {
            var isSuccessful = false;
            if (!nexBotSettings.DisabledSendSkype)
            {
                var request = await CreateRequestAsync("/messages/forward", HttpMethod.Post);
                var data = new Dictionary<string, string>
                {
                    { "conversationId", conversationId },
                    { "message", message }
                };

                request.Content = new FormUrlEncodedContent(data);

                using var response = await httpClient.SendAsync(request);
                isSuccessful = response.IsSuccessStatusCode;
            }

            return isSuccessful;
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

        private async Task SendApproveMessageAsync(string conversationId, long requestId, string message)
        {
            if (!nexBotSettings.DisabledSendSkype)
            {
                var data = new Dictionary<string, string>
                {
                    {"conversationId", conversationId },
                    {"requestId", requestId.ToString() },
                    {"message", message },
                    {"callBackUrl", $"{BaseUrl}/api" },
                };

                await SendHttpClientAsync<Task>("/request/approve", HttpMethod.Post, data);
            }
        }

        public async Task<IEnumerable<EmployeeSkypeAccount>> GetEmployeeSkypeAccountAsync(GetEmployeeSkypeAccountCriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                var employeeAccounts = await nexbotRepository.GetEmployeeSkypeAccount(criteria);

                var conversationUsers = await GetSkypeAccountAsync();

                return CombineEmployeeSkypeAccount(employeeAccounts, conversationUsers);
            }

            return new List<EmployeeSkypeAccount>();
        }

        private static IList<EmployeeSkypeAccount> CombineEmployeeSkypeAccount(IEnumerable<EmployeeSkypeAccount> employeeAccounts, IList<ConversationUser> conversationUsers)
        {
            foreach (var account in employeeAccounts)
            {
                var conversation = conversationUsers.FirstOrDefault(t => t.ConversationId == account.ConversationId);
                if (conversation != null)
                {
                    account.SkypeDisplayName = conversation.UserName;
                }
            }

            return employeeAccounts.ToList();
        }

        public async Task SendMessageForLeaveRequest(LeaveRequestViewModel request)
        {
            if (request != null)
            {
                IEnumerable<Employee> leaders;
                if (request.IsAnnualLeaveOver5Day || request.IsUnpaidLeave)
                {
                    leaders = await baseService.GetDataAsync<Employee>(new GetManagementsCriteria());
                }
                else
                {
                    var criteria = new GetLeadersOfEmployeeCriteria
                    {
                        EmployeeId = request.EmployeeId
                    };
                    leaders = await baseService.GetDataAsync<Employee>(criteria);
                }
                await SendMessageToHandleLeaveRequestMembers(leaders, request);
            }
        }

        private async Task SendMessageToHandleLeaveRequestMembers(IEnumerable<Employee> leaders, LeaveRequestViewModel request)
        {
            if (leaders != null && leaders.Any())
            {
                foreach (var leader in leaders)
                {
                    var message = GenerateMessageForLeaveRequest(request, leader.NickName);
                    await SendApproveMessageAsync(leader.ConversationId, request.Id, message);
                }
            }
        }

        private string GenerateMessageForLeaveRequest(LeaveRequestViewModel request, string leaderName)
        {
            var forDate = GetDateStringForLeaveRequest(request.FromDate, request.ToDate, request.TotalDays);
            var message = $"Dear {leaderName}, {Constants.NewLine}";
            message += $"This is inform you about your member, {request.Employee} had a leave request {Constants.NewLine}";
            message += $"_- Date: {forDate}_ {Constants.NewLine}";
            message += $"_- Type: {request.LeaveTypeName}_ {Constants.NewLine}";
            message += $"_- Remark: {request.Remark}_ {Constants.NewLine}";
            message += $"Please process this request at: {BaseUrl}/request/member.  {Constants.NewLine}";
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
            var employeee = await baseService.GetDataEntityAsync<Employee>(new GetEmployeeSkypeAccountCriteria { EmployeeId = request.Requester.Id });
            if (employeee != null)
            {
                var message = GenerateProcessRequestMessage(request, command);
                await SendMessageAsync(employeee.ConversationId, message);
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

        private string GenerateProcessRequestMessage(RequestEntity request, string command)
        {
            var action = command == Command.Reject.EnumToString() ? $"{command.ToLowerInvariant()}ed" : $"{command.ToLowerInvariant()}d";

            var forDate = request.TotalDays > 1
                ? "from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " +
                  request.ToDate.ToDateMonthYearFormatedDate()
                : "on " + request.FromDate.ToDateMonthYearFormatedDate();
            var message = $"Dear {request.Requester.NickName},{Constants.NewLine}";
            message += $"Your {request.RequestType.Name.ToLowerInvariant()} request _{forDate}_ has been {action} by _{request.ModifiedBy.DisplayName}._{Constants.NewLine}";
            message += $"For more detail, please visit: {BaseUrl}. Thanks";
            return message;
        }

        public async Task SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request, IList<Employee> leaders)
        {
            try
            {
                foreach (var leader in leaders)
                {
                    var message = GenerateCheckInOutRequestMessage(request, leader.NickName);
                    await SendApproveMessageAsync(leader.ConversationId, request.Id, message);
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
                var conversations = await scheduledMessageRepository.GetConversationsAsync(new GetConversationsCriteria { EmployeeId = lateInEarlyOutModel.EmpId }).ConfigureAwait(false);
                if (conversations.Any())
                {
                    string message = string.Format(Messages.MessageAutoDeduct, lateInEarlyOutModel.NickName);
                    result = await SendMessageDeductToConversation(conversations.Where(x =>
                        x.ConversationName.ToLower(CultureInfo.InvariantCulture).Equals(lateInEarlyOutModel.NickName.ToLower(CultureInfo.InvariantCulture)))
                        .Select(m => m.SkypeConversationId), message);
                }
            }
            catch (Exception ex)
            {
                logService.LogException(ex);
            }
            return result;
        }

        public async Task<CommandResult> SendMessageScheduledVegetarianToAllEmp(string message)
        {
            var result = new CommandResult();
            try
            {
                var conversations = await scheduledMessageRepository.GetEmployeeConversationAsync(new GetEmployeeConversation()).ConfigureAwait(false);
                if (conversations.Any())
                {
                    result = await SendMessageDeductToConversation(conversations.Select(m => m.SkypeConversationId), message);
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
                var conversations = await scheduledMessageRepository.GetTeamHrConversationsAsync(new GetTeamHrConversationCriteria()).ConfigureAwait(false);
                var users = String.Join(", ", lateInEarlyOutModels.Select(x => x.NickName));
                foreach (var item in conversations)
                {
                    var message = string.Format(Messages.MessageHRAutoDeduct, item.ConversationName, users);
                    result = await SendMessageAsync(item.SkypeConversationId, message).ConfigureAwait(false);
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
            message += $"This is to inform you about your member, {request.Employee} had a {type} request:{Constants.NewLine}";
            message += $"_- Date: {request.WorkDate.ToDateMonthYearFormatedDate()}_{Constants.NewLine}";
            message += $"_- Shift: {request.Shift}_{Constants.NewLine}";
            message += $"_- Time: {time}_{Constants.NewLine}";
            message += $"Please process this request at: {BaseUrl}/request/member. {Constants.NewLine}";
            message += $"Any questions, please contact HR department. Thanks";
            return message;
        }

        private async Task<T> SendHttpClientAsync<T>(string endPoint, HttpMethod method, IDictionary<string, string> data = null)
        {
            ValidateEndPoint(endPoint);

            using var request = await CreateRequestAsync(endPoint, method);
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
    }
}