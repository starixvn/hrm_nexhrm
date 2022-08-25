namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;

    public class TeamRepository : ITeamRepository
    {
        public async Task<Team> GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = await db.QueryEntityAsync<Team, Team, Department, Employee, Employee, Team>(
                    (t, t1, dep, x, y) =>
                    {
                        t.Parent = t1;
                        t.Department = dep;
                        t.Leader = x;
                        t.AssistantLeader = y;
                        return t;
                    },
                    "ParentId, DepartmentId, LeaderId, AssistantLeaderId", 
                    criteria);
                return entity;
            }
        }

        public async Task<IList<Team>> GetAll(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entity = await db.QueryAsync<Team, Employee, Employee, Team>(
                   (t, x, y) =>
                   {
                       t.Leader = x;
                       t.AssistantLeader = y;
                       return t;
                   },
                   "LeaderId, AssistantLeaderId",
                   criteria);

                return entity.ToList();
            }
        }
    }
}
