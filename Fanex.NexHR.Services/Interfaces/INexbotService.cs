namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.SkypeAccount;
    using Models.Request;
    using Models.SkypeAccount;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface INexbotService
    {
        void SendMessage(string conversationId, string message);

        void SendApproveMessage(string conversationId, long requestId, string message);

        IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(GetEmployeeSkypeAccountCriteria criteria);

        void SendMessageForLeaveRequest(LeaveRequestViewModel request);

        void SendMessageProcessRequest(RequestEntity request, string command);

        void SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request);
    }
}