// --------------------------------------------------------------------------
// <copyright file="CustomRoleProvider.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Web.Providers
{
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Models;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Web.Security;

    /// <summary>
    /// Custom role provider.
    /// </summary>
    public class CustomRoleProvider : RoleProvider
    {
        private readonly IEmployeeService _employeeService;

        public CustomRoleProvider(IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public CustomRoleProvider()
            : this(null)
        {
        }

        public override string ApplicationName
        {
            get { return "NexHR"; }

            set { throw new NotImplementedException(); }
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
            var roles = new List<string>
            {
                Role.Member.EnumToString(),
                Role.SpecialUser.EnumToString()
            };

            var user = GetCurrentUser(username);

            if (user.HasRolePM)
            {
                roles.Add(Role.PM.EnumToString());
            }

            if (user.RoleList != null)
            {
                foreach (var item in user.RoleList)
                {
                    switch (item)
                    {
                        default:
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
            var getEmployeeCriteria = new GetEmployeeByDomainNameCriteria { DomainUserName = username };
            var user = _employeeService.GetEmployeeByDomainName(getEmployeeCriteria);

            return user;
        }
    }
}