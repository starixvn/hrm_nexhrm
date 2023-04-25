namespace Fanex.NexHR.Criteria.CourseAndCertificate
{
    using Fanex.Data.Repository;

    public class GetCourseAndCertificateByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "CourseAndCertificate_GetById";
        }

        public override bool IsValid() => Id > 0;
    }
}
