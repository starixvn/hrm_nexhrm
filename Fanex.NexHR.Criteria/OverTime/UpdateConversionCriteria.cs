namespace Fanex.NexHR.Criteria.Overtime
{
    using System;

    public class UpdateConversionCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public float Deduct { get; set; }

        public string Description { get; set; }

        public DateTime DeductDate { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_UpdateConversion";
        }

        public override bool IsValid()
        {
            return
                Id > 0
                && ModifiedBy > 0
                && Deduct >= 0;
        }
    }
}