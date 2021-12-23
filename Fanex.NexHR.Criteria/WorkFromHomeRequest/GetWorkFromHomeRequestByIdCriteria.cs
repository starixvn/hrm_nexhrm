using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    public class GetWorkFromHomeRequestByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "WorkFromHomeRequest_GetById";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
