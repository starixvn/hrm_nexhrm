namespace Fanex.NexHR.Criteria.EmployeeExperience
{
    using Fanex.Data.Repository;
    using System;

    public class GetEmployeeExperienceCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public int EmpId { get; set; }

        public int Type { get; set; }

        public int Name { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeExperience_Get";
        }

        public override bool IsValid()
        {
            return ToDate >= FromDate;
        }
    }
}
