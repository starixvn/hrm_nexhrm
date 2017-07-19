namespace Fanex.NexHR.Criteria.LeaveType
{
    using Data;
    using Data.Repository;

    public class GetLeaveTypeCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        private bool _excludeNotShown = true;

        public bool ExcludeNotShown
        {
            get
            {
                return _excludeNotShown;
            }

            set 
            {
                _excludeNotShown = value;
            }
        }

        public override string GetSettingKey()
        {
            return "LeaveType_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
