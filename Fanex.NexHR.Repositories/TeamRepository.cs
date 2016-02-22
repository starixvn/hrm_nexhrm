namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Common.Helper;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

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

        public bool UpdateTeam(ICriteria criteria)
        {
            bool updated = false;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                updated = db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }

            return updated;
        }

        public IList<Team> GetTeamManagedByEmp(ICriteria criteria)
        {
            var entityList = new List<Team>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                entityList = db.Query<Team>(criteria.GetSpParams()).ToList();
            }

            return entityList;
        }

        public IList<Team> GetSubTeamsOfEmployee(ICriteria criteria)
        {
            var entityList = new List<Team>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                entityList = db.Query<Team>(criteria.GetSpParams()).ToList();
            }

            return entityList;
        }

        public int AddNewTeam(ICriteria criteria)
        {
            int status = -1;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                status = (int)db.ExecuteScalar(criteria.GetSpParams());
            }

            return status;
        }
    }
}
