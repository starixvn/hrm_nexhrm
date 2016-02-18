// --------------------------------------------------------------------------
// <copyright file="TeamRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.6">Apply Nap Framwork</change>
// </history>

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

    /// <summary>
    /// Team repository.
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        /// <summary>
        /// Get Team By ID.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Team entity.</returns>
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

        /// <summary>
        /// Get all Team.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team Entity.</returns>
        public IList<Team> GetAll(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader())
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

        /// <summary>
        /// Update Team.
        /// </summary>
        /// <param name="criteria">Criteria to update DB.</param>
        /// <returns>Boolean to check.</returns>
        public bool UpdateTeam(ICriteria criteria)
        {
            bool updated = false;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                updated = db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }

            return updated;
        }

        /// <summary>
        /// Get Team By Manager Id.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team.</returns>
        public IList<Team> GetTeamManagedByEmp(ICriteria criteria)
        {
            var entityList = new List<Team>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                entityList = db.Query<Team>(criteria.GetSpParams()).ToList();
            }

            return entityList;
        }

        /// <summary>
        /// Get Sub Team Of Employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team.</returns>
        public IList<Team> GetSubTeamsOfEmployee(ICriteria criteria)
        {
            var entityList = new List<Team>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                entityList = db.Query<Team>(criteria.GetSpParams()).ToList();
            }

            return entityList;
        }

        /// <summary>
        /// Add new team.
        /// </summary>
        /// <param name="criteria">Criteria to insert.</param>
        /// <returns>The result.</returns>
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
