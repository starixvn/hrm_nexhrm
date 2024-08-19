namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Data.Repository;

    public class GetEmployeeByModifiedDateCriteria : CriteriaBase
    {
        public DateTime ModifiedDate { get; set; }

        public override bool IsValid() => ModifiedDate > DateTimeOffset.MinValue;

        public override string GetSettingKey() => "Employee_GetByModifiedDate";
    }
}