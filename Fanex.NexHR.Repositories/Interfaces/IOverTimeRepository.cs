using Fanex.Data.Repository;
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
        bool Insert(ICriteria createCriteria);


        IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model);

        bool ApproveRequest(int id, int approveUserId);

        bool VerifyRequest(int id, int verifyUserId);

        OverTime Get(int id);

        bool Update(OverTimeViewModel model);
    }
}
