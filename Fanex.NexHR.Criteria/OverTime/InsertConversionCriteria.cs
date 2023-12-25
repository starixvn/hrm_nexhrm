namespace Fanex.NexHR.Criteria.Overtime
{
    using System;

    public class InsertConversionCriteria : BaseCommand
    {
        public int CreatedBy { get; set; }

        public int EmpId { get; set; }

        public float Deduct { get; set; }

        public DateTime DeductDate { get; set; }

        public string Description { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_InsertConversion";
        }

        public override bool IsValid()
        {
            return
                CreatedBy > 0
                && EmpId > 0
                && Deduct >= 0;
        }
    }
}
