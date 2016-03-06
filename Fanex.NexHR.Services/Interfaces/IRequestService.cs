namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.CheckInOutRequest;

    public interface IRequestService
    {
        CommandResult Process(string command, long id, int processBy);

        IList<RequestEntity> GetRequest(GetRequestCriteria criteria);

        RequestEntity GetRequestEntity(GetRequestByIdCriteria criteria);

        IList<RequestEntity> GetRequestHistory(GetRequestHistoryCriteria criteria);

        decimal GetNumberRequestInMonth(int employeeId, RequestType requestType);

        IList<DateTime> GetDisableRequestDate(int employeeId);

        DateRange GetDateRange(GetDateRangeEmployeeCalendarCriteria criteria);

        void SendMailForCheckInOut(CheckInOutRequestViewModel model);

        void SendMailForLeave(LeaveRequestViewModel model);
    }
}
