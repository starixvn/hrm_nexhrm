namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Conversation;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Models.Staff;
    using Fanex.NexHR.Models.TimeSheet;
    using Models.Request;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface INexbotService
    {
        Task<bool> SendMessageAsync(string conversationId, string message);

        Task SendRemindCheckInCheckOutAsync(MissingCheckInOutEmployee missingCheckInOutEmployee);

        Task<IList<ConversationUser>> GetIgnoredSkypeAccountByMessageCategoryAsync(MessageCategory messageCategory);

        Task<IList<ConversationUser>> GetRegisteredConversationAsync(MessageCategory messageCategory);

        Task SendRegistVegetarianMealAsync(string conversationId, int empId, int companyId, DateTime onDate, string message, string messageContent);

        Task SendMessageForLeaveRequest(LeaveRequestViewModel request, IEnumerable<EmployeeSupervisor> employeeSupervisors);

        Task SendMessageProcessRequest(RequestEntity request, string command);

        Task SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request, IEnumerable<EmployeeSupervisor> employeeSupervisors);

        Task<CommandResult> SendMessageDeductLateInEarlyOutToEmp(LateInEarlyOutModel lateInEarlyOutModel);

        Task<CommandResult> SendMessageDeductLateInEarlyOutToHrTeam(IEnumerable<LateInEarlyOutModel> lateInEarlyOutModels);

        Task<CommandResult> SendMessageDeductToConversation(IEnumerable<string> conversations, string message);

        Task<CommandResult> SendMessageReminderLateInEarlyOutMoreThanSixtyMinuteToEmp(IEnumerable<ReminderLateInEarlyOut> reminderLateInEarlyOuts);

        Task SendCheckoutNotification(CheckInCheckOutNotifyRecord checkoutRecord);

        Task SendCheckinNotification(CheckInCheckOutNotifyRecord checkoutRecord);

        Task SendReachedWorkingHoursNotification(WorkingHour workingHour);

        Task SendMultipleRequestsToApprover(ApproverRequestSumup approver);
    }
}