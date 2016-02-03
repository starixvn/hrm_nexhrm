namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;

    public class RejectRequestCriteria : CriteriaBase
    {
        public int Status { get; set; }

        public int Id { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_RejectRequest";
        }        
    }
}
