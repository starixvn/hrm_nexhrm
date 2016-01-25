using Fanex.NexHR.Core.Enums;
using Fanex.NexHR.Models;
using Fanex.NexHR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Services.Interfaces
{
    public interface IOverTimeService
    {
        bool Insert(OverTimeViewModel model);

        IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model);

        bool ApproveRequest(int id, int updatedUserId);

        bool VerifyRequest(int id, int verifiedUserId);

        OverTime Get(int id);

        bool Update(OverTimeViewModel model);
    }
}
