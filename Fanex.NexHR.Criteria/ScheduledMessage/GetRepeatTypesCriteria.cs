using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    public class GetRepeatTypesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "RepeatType_GetAll";

        public override bool IsValid() => true;
    }
}
