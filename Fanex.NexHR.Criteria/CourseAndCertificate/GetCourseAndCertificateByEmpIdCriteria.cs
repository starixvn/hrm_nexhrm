namespace Fanex.NexHR.Criteria.CourseAndCertificate
{
    using Fanex.Data.Repository;

    public class GetCourseAndCertificateByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "CourseAndCertificate_GetByEmpId";
        }

        public override bool IsValid() => EmpId > 0;
    }
}
