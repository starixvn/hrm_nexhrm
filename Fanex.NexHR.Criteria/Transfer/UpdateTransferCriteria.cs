namespace Fanex.NexHR.Criteria.Transfer
{
    using System;

    public class UpdateTransferCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int EmpId { get; set; }

        public int GroupId { get; set; }

        public int OldGroupId { get; set; }

        public DateTime TransferDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Transfer_Update";

        public override bool IsValid() => Id > 0  && EmpId > 0 && ModifiedBy > 0 && IsValidGroup();

        private bool IsValidGroup() => GroupId > 0 && OldGroupId > 0;
    }
}
