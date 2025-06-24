using System;
using Fanex.NexHR.Core.XML;

namespace Fanex.NexHR.Models.Bio;

[Serializable]
[XMLName("Row")]
public class TimeRecordAPI
{
    public string EmpNo { get; set; }
    public int? InOutType { get; set; }
    public DateTime RecordTime { get; set; }
    public int ReaderId { get; set; }
    public string ReaderName { get; set; }
    public string EventName { get; set; }
}