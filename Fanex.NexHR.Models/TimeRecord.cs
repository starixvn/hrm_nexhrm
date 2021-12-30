namespace Fanex.NexHR.Models
{
    using System;
    using Fanex.NexHR.Core;
    using Resources;

    public class TimeRecord
    {
        public int RecordTimeId { get; set; }

        public Employee Employee { get; set; }

        public DateTime RecordTime { get; set; }

        public short InOutType { get; set; }

        public string ReaderName { get; set; }

        public string EventName { get; set; }
        public bool IsCheckInOutOnline { get; set; }
        public string IPAddress { get; set; }
        public byte Status { get; set; }

        public string GetInOutName
        {
            get
            {
                switch (InOutType)
                {
                    case 0:
                        return Labels.In;

                    case 1:
                        return Labels.Out;

                    default:
                        return "None";
                }
            }
        }

        public string GetStatusName
        {
            get
            {
                switch (Status)
                {
                    case 1:
                        return Labels.Pending;
                    case Constants.valid:
                        return Labels.Valid;
                    case Constants.invalid:
                        return Labels.Invalid;
                    default:
                        return "None";
                }
            }
        }
    }
}