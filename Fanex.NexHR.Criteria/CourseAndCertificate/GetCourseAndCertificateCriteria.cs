namespace Fanex.NexHR.Criteria.CourseAndCertificate
{
    using Fanex.Data.Repository;
    using System;

    public class GetCourseAndCertificateCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public int? EmpId { get; set; }

        public int? TypeId { get; set; }

        public string Name { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "CourseAndCertificate_Get";
        }

        public override bool IsValid()
        {
            return ToDate >= FromDate;
        }
    }
}
