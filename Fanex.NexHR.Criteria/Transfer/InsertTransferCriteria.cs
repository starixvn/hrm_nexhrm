namespace Fanex.NexHR.Criteria.Transfer
{
    using System;

    public class InsertTransferCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public int GroupId { get; set; }

        public DateTime TransferDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Transfer_Insert";

        public override bool IsValid() => EmpId > 0 && GroupId > 0 && ModifiedBy > 0;
    }
}
