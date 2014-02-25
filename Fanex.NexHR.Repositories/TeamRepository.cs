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
        /// Update team.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public bool UpdateTeam(TeamViewModel model)
        {
            using (var db = new ObjectDb("EmployeeGroup_Update"))
            {
                db.AddInParameter("@Id", DbType.Int32, model.Id);
                db.AddInParameter("@Code", DbType.String, model.TeamCode);
                db.AddInParameter("@Name", DbType.String, model.TeamName);
                db.AddInParameter("@LeaderId", DbType.Int32, model.LeaderId);
                db.AddInParameter("@AssistantLeaderId", DbType.Int32, model.AssistantLeaderId);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, model.ModifiedBy);

                try
                {
                    return db.ExecuteNonQuery() != 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get team managed by employee.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetTeamManagedByEmp(int managerId)
        {
            using (var db = new ObjectDb("GetTeamManagedById"))
            {
                var entityList = db.Query<Team>(new { managerId }).ToList();
                return entityList;
            }
        }

        /// <summary>
        /// Get sub teams of employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetSubTeamsOfEmployee(int employeeId)
        {
            using (var db = new ObjectDb("Employee_GetSubTeam"))
            {
                var entityList = db.Query<Team>(new { empId = employeeId }).ToList();
                return entityList;
            }
        }

        /// <summary>
        /// Add new team.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public int AddNewTeam(TeamViewModel model)
        {
            using (var db = new ObjectDb("EmployeeGroup_Insert"))
            {
                db.AddInParameter("@Code", DbType.String, model.TeamCode);
                db.AddInParameter("@Name", DbType.String, model.TeamName);
                db.AddInParameter("@LeaderId", DbType.Int32, model.LeaderId);
                db.AddInParameter("@AssistantLeaderId", DbType.Int32, model.AssistantLeaderId);
                db.AddInParameter("@CreatedDate", DbType.DateTime, model.CreatedDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, model.CreatedBy);

                return (int)db.ExecuteScalar();
            }
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
        public bool DeleteTeam(int id, DateTime modifiedDate, int modifiedBy, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("EmployeeGroup_Delete"))
            {
                db.AddInParameter("@Id", DbType.Int32, id);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar() == null ? false : true;

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        /// <summary>
        /// Get teams handle requests of employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetTeamsHandleRequestsOfEmployee(int employeeId)
        {
            using (var db = new ObjectDb("Employee_GetGroupHandleRequest"))
            {
                var entityList = db.Query<Team>(new { id = employeeId }).ToList();
                return entityList;
            }
        }
    }
}
