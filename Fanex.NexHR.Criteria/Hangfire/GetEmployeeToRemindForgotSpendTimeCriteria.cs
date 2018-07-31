namespace Fanex.NexHR.Criteria.Hangfire
{
    using Data.Repository;

    public class GetEmployeeToRemindForgotSpendTimeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NexBot_ GetEmployeeToRemindForgotSpendTime";

        public override bool IsValid() => true;
    }
}