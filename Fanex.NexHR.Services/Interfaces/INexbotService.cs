namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.SkypeAccount;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Models.Request;
    using Models.SkypeAccount;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface INexbotService
    {
        Task<bool> SendMessageAsync(string conversationId, string message);

        Task<IList<ConversationUser>> GetSkypeAccountAsync();

        Task<IList<ConversationUser>> GetIgnoredSkypeAccountByMessageCategoryAsync(MessageCategory messageCategory);

        Task<IList<ConversationUser>> GetRegisteredConversationAsync(MessageCategory messageCategory);

        Task<IEnumerable<EmployeeSkypeAccount>> GetEmployeeSkypeAccountAsync(GetEmployeeSkypeAccountCriteria criteria);

        Task SendRegistVegetarianMealAsync(string conversationId, int empId, int companyId, DateTime onDate, string message);

        Task SendMessageForLeaveRequest(LeaveRequestViewModel request);

        Task SendMessageProcessRequest(RequestEntity request, string command);

        Task SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request, IList<Employee> leaders);
        Task<CommandResult> SendMessageDeductLateInEarlyOutToEmp(LateInEarlyOutModel lateInEarlyOutModel);
        Task<CommandResult> SendMessageDeductLateInEarlyOutToHrTeam(IEnumerable<LateInEarlyOutModel> lateInEarlyOutModels);
        Task<CommandResult> SendMessageDeductToConversation(IEnumerable<string> conversations, string message);
        Task<CommandResult> SendMessageReminderLateInEarlyOutToEmp(IEnumerable<ReminderLateInEarlyOut> reminderLateInEarlyOuts);
        Task<CommandResult> SendMessageReminderLateInEarlyOutToHr(IEnumerable<ReminderLateInEarlyOut> reminderLateInEarlyOuts);
    }
}