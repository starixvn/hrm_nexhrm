using System;

namespace BIO.WebAPI.Models;

public class TimeRecord
{
    public string EmpNo { get; set; }
    public byte? InOutType { get; set; }
    public DateTime RecordTime { get; set; }
    public int ReaderId { get; set; }
    public string ReaderName { get; set; }
    public string EventName { get; set; }
}