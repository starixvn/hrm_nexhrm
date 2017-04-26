namespace Fanex.NexHR.Models
{
    using System;
    using Resources;

    public class TimeRecord
    {
        public int RecordTimeId { get; set; }

        public Employee Employee { get; set; }

        public DateTime RecordTime { get; set; }

        public short InOutType { get; set; }

        public string GetInOutName
        {
            get
            {
                if (InOutType == 0)
                {
                    return Labels.In;
                }
                else if (InOutType == 1)
                {
                    return Labels.Out;
                }

                return "None";
            }
        }
    }
}
