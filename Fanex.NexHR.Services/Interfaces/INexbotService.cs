namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.SkypeAccount;
    using Models.Request;
    using Models.SkypeAccount;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface INexbotService
    {
        Task SendMessage(string conversationId, string message);

        IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(GetEmployeeSkypeAccountCriteria criteria);

        Task SendMessageForLeaveRequest(LeaveRequestViewModel request);

        Task SendMessageProcessRequest(RequestEntity request, string command);

        Task SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request);
    }
}