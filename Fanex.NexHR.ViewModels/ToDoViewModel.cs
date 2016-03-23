namespace Fanex.NexHR.ViewModels
{
    using Fanex.NexHR.Models.Request;
    using System.Collections.Generic;

    public class ToDoViewModel
    {
        public IList<MissingCheckInOutModel> MissingCheckInOutList { get; set; }

        public IList<RequestEntity> PendingRequestsList { get; set; }
    }
}
