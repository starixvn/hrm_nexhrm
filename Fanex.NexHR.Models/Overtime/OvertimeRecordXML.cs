
namespace Fanex.NexHR.Models.Overtime
{
    using System;
    using Fanex.NexHR.Core.XML;

    [Serializable]
    [XMLName("OvertimeRecord")]
    public class OvertimeRecordXML
    {
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public float Hour { get; set; }

        public float NexHour { get; set; }

        public string Note { get; set; }
    }
}
