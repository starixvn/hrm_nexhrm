using System;
using Fanex.NexHR.Core.XML;

namespace Fanex.NexHR.Models.Bio;

[Serializable]
[XMLName("Row")]
public class TimeRecordAPI
{
    public string EmpNo { get; set; }
    public byte? InOutType { get; set; }
    public DateTime RecordTime { get; set; }
}