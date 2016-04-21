namespace Fanex.NexHR.Criteria.CompanyEvent
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteEventCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "CompanyEvent_Delete";
        }
    }
}
