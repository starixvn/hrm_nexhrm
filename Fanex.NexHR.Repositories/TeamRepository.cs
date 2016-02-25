namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class TeamRepository : ITeamRepository
    {
        public Team GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<Team, Team, Department, Employee, Employee, Team>(
                    (t, t1, dep, x, y) =>
                    {
                        t.Parent = t1;
                        t.Department = dep;
                        t.Leader = x;
                        t.AssistantLeader = y;
                        return t;
                    },
                    "ParentId, DepartmentId, LeaderId, AssistantLeaderId", 
                    criteria.GetSpParams());
                return entity;
            }
        }

        public IList<Team> GetAll(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.Query<Team, Employee, Employee, Team>(
                   (t, x, y) =>
                   {
                       t.Leader = x;
                       t.AssistantLeader = y;
                       return t;
                   },
                   "LeaderId, AssistantLeaderId",
                   criteria.GetSpParams());

                return entity.ToList();
            }
        }
    }
}
