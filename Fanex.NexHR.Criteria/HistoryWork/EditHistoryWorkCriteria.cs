namespace Fanex.NexHR.Criteria.HistoryWork
{
    using Fanex.Data.Repository;

    public class EditHistoryWorkCriteria : CriteriaBase
    {
        public int Id { get; set; }     

        public override string GetSettingKey()
        {
            return "HistoryWork_GetJobtitleAndTeam";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
