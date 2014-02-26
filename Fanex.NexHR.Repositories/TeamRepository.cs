// --------------------------------------------------------------------------
// <copyright file="TeamRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
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
    using Fanex.NAP.Framework.Data;
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
        public Team Get(ICriteria criteria)
        {
            var team = new Team();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                    {
                        while (reader.Read())
                        {
                            team = new Team
                            {
                                Id = reader.ToInt("Id"),
                                Code = reader.ToString("Code"),
                                Name = reader.ToString("Name"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                                Leader = new Employee
                                {
                                    NickName = reader.ToString("LeaderNickName"),
                                    FullName = reader.ToString("LeaderFullName"),
                                    Id = reader.ToInt("LeaderId"),
                                },
                                AssistantLeader = new Employee
                                {
                                    NickName = reader.ToString("AssistantLeaderNickName"),
                                    FullName = reader.ToString("AssistantLeaderFullName"),
                                    Id = reader.ToInt("AssistantLeaderId")
                                }
                            };
                        }
                    }
                }
            }

            return team;
        }

        /// <summary>
        /// Get all Team.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team Entity.</returns>
        public IList<Team> GetAll(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Team> entityList = new List<Team>();
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    using (IDataReader reader = db.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                var entity = new Team
                                {
                                    Id = reader.ToInt("Id"),
                                    Code = reader.ToString("Code"),
                                    Name = reader.ToString("Name"),
                                    Leader = new Employee
                                    {
                                        NickName = reader.ToString("LeaderNickName"),
                                        FullName = reader.ToString("LeaderFullName"),
                                        Id = reader.ToInt("LeaderId"),
                                    },
                                    AssistantLeader = new Employee
                                    {
                                        NickName = reader.To<string>("AssistantLeaderNickName"),
                                        FullName = reader.To<string>("AssistantLeaderFullName"),
                                        Id = reader.To<int>("AssistantLeaderId")
                                    }
                                };

                                entityList.Add(entity);
                            }
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Update Team.
        /// </summary>
        /// <param name="criteria">Criteria to update DB.</param>
        /// <returns>Boolean to check.</returns>
        public bool UpdateTeam(ICriteria criteria)
        {
            int updated = 0;
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    updated = db.ExecuteNonQuery(criteria.GetSpParams());
                }
            }

            if (updated != 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get Team By Manager Id.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team.</returns>
        public IList<Team> GetTeamManagedByEmp(ICriteria criteria)
        {
            var entityList = new List<Team>();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    entityList = db.Query<Team>(criteria.GetSpParams()).ToList();
                }
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
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    entityList = db.Query<Team>(criteria.GetSpParams()).ToList();
                }
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
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    status = (int)db.ExecuteScalar(criteria.GetSpParams());
                }
            }

            return status;
        }

        /// <summary>
        /// Delete team.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>The result.</returns>
        public bool DeleteTeam(ICriteria criteria, out int errorCode, out string message)
        {
            message = string.Empty;
            errorCode = 0;
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    db.ExecuteScalar(criteria.GetSpParams());

                    message = Convert.ToString(db.GetParameterValue("ErrMsg"));
                    errorCode = (int)db.GetParameterValue("ErrCode");
                    if (errorCode != 0)
                    {
                        return false;
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get teams handle requests of employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetTeamsHandleRequestsOfEmployee(ICriteria criteria)
        {
            var entityList = new List<Team>();
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    entityList = db.Query<Team>(criteria.GetSpParams()).ToList();
                }
            }

            return entityList;
        }
    }
}
