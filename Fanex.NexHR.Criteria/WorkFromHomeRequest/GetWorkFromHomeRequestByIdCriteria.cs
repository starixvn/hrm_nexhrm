namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    using Fanex.Data.Repository;

    public class GetWorkFromHomeRequestByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "WorkFromHomeRequest_GetById";

        public override bool IsValid() => Id > 0;
    }
}
