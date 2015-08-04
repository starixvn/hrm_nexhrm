// --------------------------------------------------------------------------
// <copyright file="IRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IRequestService
    {
        bool CheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool Leave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        bool UpdateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool UpdateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        CommandResult Process(string command, long id, int processBy);

        IList<RequestEntity> GetRequest(GetRequestCriteria criteria);

        RequestEntity GetRequestEntity(GetRequestByIdCriteria criteria);

        bool DeleteRequest(DeleteRequestCriteria criteria);

        IList<RequestEntity> GetRequestHistory(GetRequestHistoryCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);
    }
}
