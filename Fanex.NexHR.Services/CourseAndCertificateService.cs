namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.CourseAndCertificate;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.TrainingAndDevelopment;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CourseAndCertificateService : ICourseAndCertificateService
    {
        private readonly IBaseService baseService;

        public CourseAndCertificateService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<IEnumerable<CourseAndCertificate>> GetAll(GetCourseAndCertificateCriteria criteria)
        {
            var courseAndCertificate = await baseService.GetDataAsync<CourseAndCertificate>(criteria);
            return courseAndCertificate.ToList();
        }

        public async Task<CourseAndCertificate> GetById(GetCourseAndCertificateByIdCriteria criteria)
        {
            var courseAndCertificate = await baseService.GetDataAsync<CourseAndCertificate>(criteria);
            return courseAndCertificate.First();
        }

        public async Task<IList<CourseAndCertificate>> GetByEmpId(GetCourseAndCertificateByEmpIdCriteria criteria)
        {
            var courseAndCertificate = await baseService.GetDataAsync<CourseAndCertificate>(criteria);
            return courseAndCertificate.ToList();
        }

        public async Task<IList<TrainingAndDevelopmentType>> GetTrainingAndDevelopmentType()
        {
            var trainingAndDevelopments = await baseService.GetDataAsync<TrainingAndDevelopmentType>(new GetTrainingAndDevelopmentType());
            return trainingAndDevelopments.ToList();
        }
    }
}
