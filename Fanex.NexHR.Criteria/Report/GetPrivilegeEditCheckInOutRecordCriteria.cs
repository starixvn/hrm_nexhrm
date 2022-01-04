
namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetPrivilegeEditCheckInOutRecordCriteria : CriteriaBase
    {
        public int EmpId { get; set; }
        public DateTime WorkDate { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_GetPrivilegeEditCheckInOut";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
