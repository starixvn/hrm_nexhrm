namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LookupData;
    using Interfaces;
    using Models;

    public class LookupDataService : ILookupDataService
    {
        private readonly IBaseService baseService;

        public LookupDataService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<LookupData>> GetAll(GetAllLookupDataCriteria criteria)
        {
            return await baseService.GetDataAsync<LookupData>(criteria);
        }

        public IEnumerable<SalaryGroup> GetSalaryGroups()
        {
            return new List<SalaryGroup>
            {
                new SalaryGroup
                {
                    SalaryGroupId = 1,
                    Name = "SD1",
                    Description = "Software Engineer, Database Administrator, Business Analyst, System Analyst, Data Analyst"
                },
                new SalaryGroup
                {
                    SalaryGroupId = 2,
                    Name = "SD2",
                    Description = "Software Test Engineer, Front-end Software Engineer, Application Support Analyst, Graphic Designer, Technical Writer, Project Support Officer"
                },
                new SalaryGroup
                {
                    SalaryGroupId = 3,
                    Name = "ITNW",
                    Description = string.Empty
                }
            };
        }
    }
}