namespace Fanex.NexHR.ViewModels.Overtime
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Overtime;

    public class OvertimeVerifyViewModel
    {
        public OvertimeVerifyViewModel()
        {
            TimeSheets = new List<OvertimeRecord>();
        }

        public int Id { get; set; }

        public string Description { get; set; }

        public string TimeSheetData { get; set; }

        public IList<OvertimeRecord> TimeSheets { get; set; }
    }
}
