namespace Fanex.NexHR.Criteria.Promotion
{
    using Fanex.Data.Repository;

    public class GetManagementGradeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "ManagementGrade_GetAll";

        public override bool IsValid() => true;
    }
}
