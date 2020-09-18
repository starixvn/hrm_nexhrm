namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class YearlyNexRequestRepository : IYearlyNexRequestRepository
    {
        public IList<YearlyNexRequest> GetYearlyNexRequest(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var yearlyNexRequest = db.Query<YearlyNexRequest, Employee, Team, YearlyNexRequest>(
                     (yearly, employee, team) =>
                     {
                         employee.Team = team;
                         yearly.Employee = employee;
                         return yearly;
                     },
                     "EmpId, GroupId",
                     criteria.GetSpParams()).ToList();

                return yearlyNexRequest;
            }
        }
    }
}
