namespace Fanex.NexHR.Criteria.Position
{
    using Data.Repository;

    public class GetAllPositionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Position_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
