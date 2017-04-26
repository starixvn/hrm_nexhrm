namespace Fanex.NexHR.Criteria
{
    using System;
    using Data.Repository;

    public class GetBirthdayAndFirstWorkingdayCriteria : CriteriaBase
    {
        public DateTime OnDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetBirthdayAndFirstWorkingday";
        }
    }
}
