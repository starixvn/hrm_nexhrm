namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateOvertimeCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public float Hour { get; set; }

        public int EmployeeId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_Update";
        }
    }
}
