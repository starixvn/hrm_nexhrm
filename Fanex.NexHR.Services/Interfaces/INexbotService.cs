namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.SkypeAccount;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Models.Request;
    using Models.SkypeAccount;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface INexbotService
    {
        bool SendMessage(string conversationId, string message);

        IList<ConversationUser> GetSkypeAccount();

        IList<ConversationUser> GetIgnoredSkypeAccountByMessageCategory(int messageCategoryId);

        IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(GetEmployeeSkypeAccountCriteria criteria);

        void SendMessageForLeaveRequest(LeaveRequestViewModel request);

        void SendMessageProcessRequest(RequestEntity request, string command);

        void SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request, IList<Employee> leaders);

        Task<IEnumerable<string>> GetRegisteredConversation(MessageCategory CategoryId);
    }
}