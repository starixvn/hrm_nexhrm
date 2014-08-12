// --------------------------------------------------------------------------
// <copyright file="ExtensionNoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.14">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// ExtensionNo Service.
    /// </summary>
    public class ExtensionNoService : BaseService, IExtensionNoService
    {
        private readonly IExtensionNoRepository _repository;

        public ExtensionNoService()
        {
            this._repository = new ExtensionNoRepository();
        }

        public IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = this._repository.GetAllExtensionNo(criteria);
                return result;
            }

            return null;
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = this._repository.GetAllExtensionGroupInfo(criteria);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Function to add new extension number.
        /// </summary>
        /// <param name="criteria">AddNewExtensionNoMember Criteria.</param>
        /// <returns>Result of added.</returns>
        public CommandResult AddNewExtensionNoMember(AddNewExtensionNoMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;

            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = this._repository.GetEmployeeExtensionNoById(criteria);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Function to update extension number.
        /// </summary>
        /// <param name="criteria">UpdateExtensionNoMember Criteria.</param>
        /// <returns>Result of updated.</returns>
        public CommandResult UpdateExtensionNoMember(UpdateExtensionNoMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;

            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        /// <summary>
        /// Function to delete extension number.
        /// </summary>
        /// <param name="criteria">DeleteExtensionNumber criteria.</param>
        /// <returns>Result of deleted.</returns>
        public CommandResult DeleteExtensionNumber(DeleteExtensionNumberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;

            if (criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }

        /// <summary>
        /// Function to add external group.
        /// </summary>
        /// <param name="criteria">AddNewExternalGroup Criteria.</param>
        /// <returns>Result of added.</returns>
        public CommandResult AddNewExternalGroup(AddNewExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;

            if (criteria.IsValid())
            {
                result = this.Repository.Insert(criteria);
            }

            return result;
        }

        /// <summary>
        /// Function to update external group.
        /// </summary>
        /// <param name="criteria">UpdateExternalGroup Criteria.</param>
        /// <returns>Result of updated.</returns>
        public CommandResult UpdateExternalGroup(UpdateExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;

            if (criteria.IsValid())
            {
                result = this.Repository.Update(criteria);
            }

            return result;
        }

        /// <summary>
        /// Function to delete external group.
        /// </summary>
        /// <param name="criteria">DeleteExternalGroup Criteria.</param>
        /// <returns>Result of deleted.</returns>
        public CommandResult DeleteExternalGroup(DeleteExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;

            if (criteria.IsValid())
            {
                result = this.Repository.Delete(criteria);
            }

            return result;
        }
    }
}
