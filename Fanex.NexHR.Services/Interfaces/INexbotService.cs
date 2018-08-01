namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.SkypeAccount;
    using Models.Request;
    using Models.SkypeAccount;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface INexbotService
    {
        Task SendMessage(string conversationId, string message);

        Task<IEnumerable<string>> GetRegisteredConversation(HangfireCategoryMessageType categoryId);

        IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(GetEmployeeSkypeAccountCriteria criteria);

        Task SendMessageForLeaveRequest(LeaveRequestViewModel request);

        Task SendMessageProcessRequest(RequestEntity request, string command);

        Task SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request);
    }
}