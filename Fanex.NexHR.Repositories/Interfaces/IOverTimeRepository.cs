using Fanex.Data.Repository;
using Fanex.NexHR.Core.Enums;
using Fanex.NexHR.Models;
using Fanex.NexHR.Models.Request;
using Fanex.NexHR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Repositories.Interfaces
{
    public interface IOverTimeRepository
    {
        bool Insert(ICriteria createCriteria);

        IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model);

        OverTime Get(int id);

        bool Update(OverTimeViewModel model);

        bool UpdateStatus(int id, RequestStatuse status, int rejectedUserId);
    }
}
