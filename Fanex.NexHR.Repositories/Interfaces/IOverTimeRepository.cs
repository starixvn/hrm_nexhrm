using Fanex.NexHR.Core.Enums;
using Fanex.NexHR.Models;
using Fanex.NexHR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Repositories.Interfaces
{
    public interface IOverTimeRepository
    {
        bool Insert(OverTimeViewModel model);


        IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model);

        bool UpdateStatus(int id, OTRequestStatus status);

        OverTime Get(int id);

        bool Update(OverTime model);
    }
}
