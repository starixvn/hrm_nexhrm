namespace Fanex.NexHR.Criteria.Report
{
    using System;

    public class EditInsertCheckInOutRecordCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public DateTime RecordTime { get; set; }

        public short InOutType { get; set; }

        public string IPAddress { get; set; }

        public string Platform { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsEditCheckInOutOnline { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_EditInsertCheckInOutTime";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}