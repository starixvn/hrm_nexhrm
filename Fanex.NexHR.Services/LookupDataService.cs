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
                    Description = @"Principle/Staff/Senior/Business Analyst;
                        Principle/Staff/Senior/Software Engineer;
                        Principle/Staff/Senior/Database Administrator;
                        Principle/Staff/Senior/Data Analyst;
                        Principle/Staff/Senior/AI Engineer;
                        Principle/Staff/Senior/BI Engineer;
                        Principle/Staff/Senior/Platform Engineer;
                        Principle/Staff/Senior/Site Reliability Engineer"
                },
                new SalaryGroup
                {
                    SalaryGroupId = 2,
                    Name = "SD2",
                    Description = @"Principle/Staff/Senior/Application Support Analyst;
                        Principle/Staff/Senior/Software Test Engineer;
                        Principle/Staff/Senior/Graphic Designer;
                        Project Support Officer;
                        Principle/Staff/Senior/Technical Writer;
                        Principle/Staff/Senior/Front-end Software Engineer;
                        Principle/Staff/Senior/Product Sound Designer;
                        Principle/Staff/Senior/Product UI Artist"
                },
                new SalaryGroup
                {
                    SalaryGroupId = 3,
                    Name = "SD3",
                    Description = "Staff/Senior/ITNW"
                }
            };
        }
    }
}