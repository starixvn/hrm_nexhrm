namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Models.Request;

    public class ToDoViewModel
    {
        public IList<MissingCheckInOutModel> MissingCheckInOutList { get; set; }

        public IList<RequestEntity> PendingRequestsList { get; set; }
    }
}
