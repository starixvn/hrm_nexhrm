namespace Fanex.NexHR.Criteria.Postnatal
{
    using Fanex.Data.Repository;
    using System;

    public class GetAllPostnatalCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeePostnatal_GetAll";
        }
    }
}
