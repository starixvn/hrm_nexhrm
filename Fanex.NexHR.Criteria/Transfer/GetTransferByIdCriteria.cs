namespace Fanex.NexHR.Criteria.Transfer
{
    using Fanex.Data.Repository;

    public class GetTransferByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "Transfer_GetById";

        public override bool IsValid() => Id > 0;
    }
}
