namespace Fanex.NexHR.Criteria.Shift
{
    using Data.Repository;

    public class GetShiftCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Shift_GetShifts";
        }
    }
}
