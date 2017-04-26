namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetUpdatingScoreRelatedPeopleCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "TOEIC_GetUpdatingScoreRelatedPeople";
        }
    }
}
