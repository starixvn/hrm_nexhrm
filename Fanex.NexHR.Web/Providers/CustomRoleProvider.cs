// --------------------------------------------------------------------------
// <copyright file="CustomRoleProvider.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Web.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Security;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Custom role provider.
    /// </summary>
    public class CustomRoleProvider : RoleProvider
    {
        private IEmployeeService _employeeService;

        public CustomRoleProvider(IEmployeeService employeeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public CustomRoleProvider()
            : this(null)
        { 
        }

        public override string ApplicationName
        {
            get
            {
                return "NexHR";
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get roles for user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>List of role.</returns>
        public override string[] GetRolesForUser(string username)
        {
            var user = GetCurrentUser(username);
            var roles = new List<string>();

            roles.Add(Role.Member.EnumToString());

            if (user.RoleList != null)
            {
                foreach (var item in user.RoleList)
                {
                    switch (item)
                    {
                        case 1:
                            roles.Add(Role.AssistantLeader.EnumToString());
                            break;

                        case 2:
                            roles.Add(Role.AssistantLeader.EnumToString());
                            roles.Add(Role.Leader.EnumToString());
                            break;

                        case 3:
                            roles.Add(Role.HR.EnumToString());
                            break;

                        case 4:
                            roles.Add(Role.AssistantLeader.EnumToString());
                            roles.Add(Role.Leader.EnumToString());
                            roles.Add(Role.HR.EnumToString());
                            roles.Add(Role.SA.EnumToString());
                            break;
                    }
                }
            }

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        private Employee GetCurrentUser(string username)
        {
            var getEmployeeCriteria = new GetEmployeeCriteria() { DomainUserName = username };
            var user = _employeeService.GetEmployeeById(getEmployeeCriteria);

            return user;
        }      
    }
}
