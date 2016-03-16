using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fanex.NexHR.Models.Notification;
using Fanex.NexHR.Services.Interfaces;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        public Notification GetViolationByEmpId(ICriteria criteria)
        {
            Notification noti = new Notification();
            noti = GetDataEntity<Notification>(criteria);

            return noti;
        }
    }
}
