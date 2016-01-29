namespace Fanex.NexHR.Criteria.OverTime
{
    using Fanex.Data.Repository;

    public class UpdateStatusCriteria : CriteriaBase
    {
        public int Status { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public override string GetSettingKey()
        {
            return "OverTime_UpdateStatus";
        }        
    }
}
