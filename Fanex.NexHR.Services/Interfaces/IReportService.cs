// --------------------------------------------------------------------------
// <copyright file="IReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.14">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;

    public interface IReportService
    {
        IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria);

        CommandResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria);
    }
}
