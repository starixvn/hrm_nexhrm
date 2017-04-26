namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Data.Repository;

    public class DeleteLateInEarlyOutCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "LateInEarlyOut_Delete";
        }
    }
}
