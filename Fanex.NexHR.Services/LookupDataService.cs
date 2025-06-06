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
                    Description = @"AI Engineer;
                        BI Engineer;
                        Business Analyst;
                        Data Analyst;
                        Database Administrator;
                        Platform Engineer;
                        Product Database Administrator;
                        Product Software Engineer;
                        Site Reliability Engineer;
                        Software Engineer"
                },
                new SalaryGroup
                {
                    SalaryGroupId = 2,
                    Name = "SD2",
                    Description = @"Application Support Analyst;
                        Front-end Software Engineer;
                        Graphic Designer;
                        Product Graphic Designer;
                        Product Sound Designer;
                        Project Support Officer;
                        Product Test Engineer;
                        Product UI Artist;
                        Product UI Designer;
                        Software Test Engineer;
                        Technical Writer"
                },
                new SalaryGroup
                {
                    SalaryGroupId = 3,
                    Name = "SD3",
                    Description = "IT Network Engineer"
                }
            };
        }
    }
}