namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.SkypeAccount;
    using Fanex.NexHR.Models;
    using Models.Request;
    using Models.SkypeAccount;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface INexbotService
    {
        bool SendMessage(string conversationId, string message);

        IList<ConversationUser> GetSkypeAccount();

        void SendApproveMessage(string conversationId, long requestId, string message);

        IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(GetEmployeeSkypeAccountCriteria criteria);

        void SendMessageForLeaveRequest(LeaveRequestViewModel request);

        void SendMessageProcessRequest(RequestEntity request, string command);

        void SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request, IList<Employee> leaders);
    }
}