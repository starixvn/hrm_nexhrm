namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.CheckInOutRequest;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public interface IRequestService
    {
        bool CreateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool CreateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        bool UpdateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool UpdateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        CommandResult Process(string command, long id, int processBy);

        IList<RequestEntity> GetRequest(GetRequestCriteria criteria);

        RequestEntity GetRequestEntity(GetRequestByIdCriteria criteria);        

        IList<RequestEntity> GetRequestHistory(GetRequestHistoryCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);
    }
}
