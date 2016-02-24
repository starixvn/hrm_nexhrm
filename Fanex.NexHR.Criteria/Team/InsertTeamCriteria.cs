// --------------------------------------------------------------------------
namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Fanex.Data.Repository;

    public class InsertTeamCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int LeaderId { get; set; }

        public int AssistantLeaderId { get; set; }

        public int? DepartmentId { get; set; }

        public int? ParentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Insert";
        }
    }
}
