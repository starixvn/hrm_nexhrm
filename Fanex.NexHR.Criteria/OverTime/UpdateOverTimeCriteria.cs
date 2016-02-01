namespace Fanex.NexHR.Criteria.OverTime
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateOverTimeCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public float Hour { get; set; }

        public int EmployeeId { get; set; }

        public int ModifyBy { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_Update";
        }
    }
}
