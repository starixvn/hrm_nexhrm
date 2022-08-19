namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Request;
    using Models;
    using Models.Request;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public interface IRequestService
    {
        Task<CommandResult> Process(string command, long id, int processBy);

        IList<RequestEntity> GetRequest(GetRequestCriteria criteria);

        RequestEntity GetRequestEntity(GetRequestByIdCriteria criteria);

        IList<RequestEntity> GetRequestHistory(GetRequestHistoryCriteria criteria);

        decimal GetNumberRequestInMonth(int employeeId, RequestType requestType);

        IList<DateTime> GetDisableRequestDate(int employeeId, long? requestId);

        DateRange GetDateRange(GetDateRangeEmployeeCalendarCriteria criteria);

        void SendMailForCheckInOut(CheckInOutRequestViewModel model, IList<Employee> leaders);

        IList<Employee> GetLeaderForCheckInOut(CheckInOutRequestViewModel model);

        void SendMailForLeave(LeaveRequestViewModel model);

        LeaveBalanceInMonth GetAnnualLeaveBalanceInMonth(GetLeaveBalanceInMonthCriteria criteria);
    }
}