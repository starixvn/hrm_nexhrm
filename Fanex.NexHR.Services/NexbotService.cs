namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Hangfire;
    using Criteria.SkypeAccount;
    using Interfaces;
    using Models.NexBot;
    using Models.Request;
    using Models.SkypeAccount;
    using Repositories;
    using Repositories.Interfaces;
    using RestSharp;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public class NexbotService : BaseService, INexbotService
    {
        private readonly IRestClient webClient;
        private readonly INexbotRepository nexbotRepository;

        public NexbotService()
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            nexbotRepository = new NexbotRepository();
        }

        public async Task SendMessage(string conversationId, string message)
        {
            var token = await GetToken().ConfigureAwait(false);
            await ForwardToBot(message, conversationId, token).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetRegisteredConversation(HangfireCategoryMessageType categoryId)
        {
            return await GetDataAsync<string>(
                new GetRegisteredConversation
                {
                    CategoryId = categoryId
                }).ConfigureAwait(false);
        }

        private async Task ForwardToBot(string message, string conversationId, Token token)
        {
            webClient.BaseUrl = NexBotConfiguration.BotServiceUrl;

            var request = new RestRequest("/messages/forward", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("message", message);
            request.AddParameter("conversationId", conversationId);

            await webClient.ExecutePostTaskAsync(request).ConfigureAwait(false);
        }

        private async Task<Token> GetToken()
        {
            webClient.BaseUrl = NexBotConfiguration.TokenUrl;

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", NexBotConfiguration.ClientId);
            request.AddParameter("client_secret", NexBotConfiguration.ClientPassword);
            request.AddParameter("scope", $"{NexBotConfiguration.ClientId}/.default");

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = await webClient.ExecutePostTaskAsync<Token>(request).ConfigureAwait(false);

            return response.Data;
        }

        public IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(GetEmployeeSkypeAccountCriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return nexbotRepository.GetEmployeeSkypeAccount(criteria);
            }

            return new List<EmployeeSkypeAccount>();
        }

        public async Task SendMessageForLeaveRequest(LeaveRequestViewModel request)
        {
            if (request != null)
            {
                var criteria = new GetLeaderSkypeAccountCriteria
                {
                    EmployeeId = request.EmployeeId
                };
                var members = nexbotRepository.GetEmployeeSkypeAccount(criteria);
                if (request.IsAnnualLeaveOver5Day || request.IsUnpaidLeave)
                {
                    var managements = nexbotRepository.GetEmployeeSkypeAccount(new GetManagementSkypeAccountCriteria());
                    members = members.Concat(managements);
                }
                await SendMessageToHandleLeaveRequestMembers(members, request).ConfigureAwait(false);
            }
        }

        private async Task SendMessageToHandleLeaveRequestMembers(IEnumerable<EmployeeSkypeAccount> members, LeaveRequestViewModel request)
        {
            if (members != null && members.Any())
            {
                foreach (var member in members)
                {
                    var message = GenerateMessageForLeaveRequest(request, member.Employee.NickName);
                    await SendMessage(member.ConversationId, message).ConfigureAwait(false);
                }
            }
        }

        private static string GenerateMessageForLeaveRequest(LeaveRequestViewModel request, string leaderName)
        {
            var forDate = GetDateStringForLeaveRequest(request.FromDate, request.ToDate, request.TotalDays);
            var message = $"Dear {leaderName}, {Constants.NewLine}";
            message += $"This is inform you about your member, {request.Employee} had a leave request {Constants.NewLine}";
            message += $"- Date: _{forDate}_ {Constants.NewLine}";
            message += $"- Type: _{request.LeaveTypeName}_ {Constants.NewLine}";
            message += $"- Remark: _{request.Remark}_ {Constants.NewLine}";
            message += $"Please process this request at: http://hrm.nexcel.vn/request/member {Constants.NewLine}.";
            message += "Any questions, please contact HR department.";
            return message;
        }

        private static string GetDateStringForLeaveRequest(DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            var forDate = totalDays > 1
                ? string.Format(
                    Constants.MailContentLeave,
                    fromDate.ToDateMonthYearFormatedDate(),
                    toDate.ToDateMonthYearFormatedDate(),
                    totalDays)
                : (totalDays == 1
                    ? string.Format(Constants.MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate())
                    : string.Format(
                        Constants.MailContentLeaveHalfDay,
                        fromDate.ToDateMonthYearFormatedDate(),
                        fromDate.ToString("HH:mm"),
                        toDate.ToString("HH:mm")));

            return forDate;
        }

        public async Task SendMessageProcessRequest(RequestEntity request, string command)
        {
            var skypeAccount = nexbotRepository.GetEmployeeSkypeAccount(new GetEmployeeSkypeAccountCriteria { EmployeeId = request.Requester.Id }).FirstOrDefault();
            if (skypeAccount != null)
            {
                var message = GenerateProcessRequestMessage(request, skypeAccount, command);
                await SendMessage(skypeAccount.ConversationId, message).ConfigureAwait(false);
            }
        }

        private static string GenerateProcessRequestMessage(RequestEntity request, EmployeeSkypeAccount skypeAccount, string command)
        {
            var action = command == Command.Reject.EnumToString() ? $"{command}ed" : $"{command}d";
            var forDate = request.TotalDays > 1
                ? " from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " +
                  request.ToDate.ToDateMonthYearFormatedDate()
                : " on " + request.FromDate.ToDateMonthYearFormatedDate();
            var message = $"Dear {skypeAccount.Employee.NickName},{Constants.NewLine}";
            message += $"Your {request.RequestType.Name} request _{forDate}_ created at _{request.CreatedDate.ToDateMonthYearFormatedDate()}_ " +
                    $"has been {action} by _{request.ModifiedBy.DisplayName}_ at _{request.ModifiedDate.ToDateMonthYearFormatedDate()}_";
            return message;
        }

        public async Task SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request)
        {
            var criteria = new GetLeaderSkypeAccountCriteria
            {
                EmployeeId = request.EmployeeId
            };
            var leaders = nexbotRepository.GetEmployeeSkypeAccount(criteria);

            if (leaders != null && leaders.Any())
            {
                var message = GenerateCheckInOutRequestMessage(request, leaders.First().Employee.NickName);
                await SendMessage(leaders.First().ConversationId, message).ConfigureAwait(false);
            }
        }

        private static string GenerateCheckInOutRequestMessage(CheckInOutRequestViewModel request, string leaderName)
        {
            string type = request.CheckInOutType == (short)RequestType.CheckIn
                            ? RequestType.CheckIn.EnumToString().ToLower(CultureInfo.InvariantCulture)
                            : RequestType.CheckOut.EnumToString().ToLower(CultureInfo.InvariantCulture);
            string time = request.Hour.ToString().PadLeft(2, '0') + ":" + request.Minute.ToString().PadLeft(2, '0');
            string message = $"Dear {leaderName}, {Constants.NewLine}";
            message += $"This is to inform you about your member, {request.Employee} had a {type} request:{Constants.NewLine}";
            message += $"- Date: _{request.WorkDate}_ ({request.Shift}){Constants.NewLine}";
            message += $"- Time: _{time}_{Constants.NewLine}";
            message += $"Please process this request at: http://hrm.nexcel.vn/request/member. {Constants.NewLine}";
            message += $"Any questions, please contact HR department. Thanks";
            return message;
        }
    }
}