using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fanex.Data.Repository;
using Fanex.NexHR.Models.Notification;

namespace Fanex.NexHR.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Notification GetViolationByEmpId(ICriteria criteria);
    }
}
