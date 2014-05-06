// --------------------------------------------------------------------------
// <copyright file="IRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Services.Interface
{
    using System.Web.Mvc;
    using Fanex.NexHR.ViewModels;

    public interface IRequestService
    {
        bool CheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool LateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        bool Leave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        bool UpdateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool UpdateLateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        bool UpdateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        bool Process(string command, long id, int processBy, out string errorMessage);
    }
}
