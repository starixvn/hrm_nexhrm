namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.SkypeAccount;

    public class NexbotRepository : BaseRepository, INexbotRepository
    {
        public async Task<IEnumerable<EmployeeSkypeAccount>> GetEmployeeSkypeAccount(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var employees = await db.QueryAsync<EmployeeSkypeAccount, Employee, Team, EmployeeSkypeAccount>(
                        (s, e, g) =>
                        {
                            s.Employee = e;
                            e.Team = g;
                            return s;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria);

                return employees;
            }
        }
    }
}