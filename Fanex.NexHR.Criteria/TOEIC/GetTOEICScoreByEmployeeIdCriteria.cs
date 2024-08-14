namespace Fanex.NexHR.Criteria.Toeic
{
    using Data.Repository;

    public class GetToeicScoreByEmployeeIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "TOEIC_GetScore";
    }
}
