namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.CourseAndCertificate;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICourseAndCertificateService
    {
        Task<IEnumerable<CourseAndCertificate>> GetAll(GetCourseAndCertificateCriteria criteria);
        Task<CourseAndCertificate> GetById(GetCourseAndCertificateByIdCriteria criteria);
        Task<IList<CourseAndCertificate>> GetByEmpId(GetCourseAndCertificateByEmpIdCriteria criteria);
    }
}
