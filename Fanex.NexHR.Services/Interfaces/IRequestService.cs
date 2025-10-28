namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Request;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Models.Staff;
    using Models;
    using Models.Request;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface IRequestService
    {
        Task<CommandResult> Process(string command, long id, int processBy);

        Task<IList<RequestEntity>> GetRequest(GetRequestCriteria criteria);

        Task<RequestEntity> GetRequestEntity(GetRequestByIdCriteria criteria);

        Task<IList<RequestEntity>> GetRequestHistory(GetRequestHistoryCriteria criteria);

        Task<decimal> GetNumberRequestInMonth(int employeeId, RequestType requestType);

        Task<IEnumerable<DateTime>> GetDisableRequestDate(int employeeId, long? requestId);

        Task<DateRange> GetDateRange(GetDateRangeEmployeeCalendarCriteria criteria);

        Task SendMailForCheckInOutAsync(CheckInOutRequestViewModel model, IEnumerable<EmployeeSupervisor> employeeSupervisors);

        Task<IEnumerable<EmployeeSupervisor>> GetLeaderForCheckInOut(int employeeId, bool isStandbyAtWeekend);

        void SendEmailAndChatBotForCheckInOut(CheckInOutRequestViewModel model);

        Task<IEnumerable<EmployeeSupervisor>> GetLeaderForLeave(int employeeId, bool isManagementApprove);

        void SendMailForLeave(LeaveRequestViewModel model, IEnumerable<EmployeeSupervisor> employeeSupervisors);

        void SendMailDeductLateInEarlyOutToEmp(LateInEarlyOutModel lateInEarlyOutModel);

        Task<LeaveBalanceInMonth> GetAnnualLeaveBalanceInMonth(GetLeaveBalanceInMonthCriteria criteria);

        Task<IEnumerable<ApproverRequestSumup>> GetPendingRequestGroupByApproverAsync();
    }
}