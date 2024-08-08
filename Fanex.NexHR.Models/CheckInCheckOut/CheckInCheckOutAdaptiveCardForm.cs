using System;

namespace Fanex.NexHR.Models.CheckInCheckOut
{
    public class CheckInCheckOutAdaptiveCardForm
    {
        public int EmpId { get; set; }
        public int MissingType { get; set; }
        public string ShiftCode { get; set; }
        public string BeginCheckInTime { get; set; }
        public string EndCheckOutTime { get; set; }
        public string Shift => $"{ShiftCode} ({BeginCheckInTime} > {EndCheckOutTime})";
        public DateTime AttendanceDate { get; set; }
    }
}