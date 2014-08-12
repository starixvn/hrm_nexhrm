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
    public class ExtensionNoService : IExtensionNoService
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
        public bool AddNewExtensionNoMember(AddNewExtensionNoMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int errorCode;
            string message;

            if (criteria.IsValid())
            {
                var result = this._repository.AddNewExtensionNoMember(criteria, out errorCode, out message);
                return result;
            }

            return false;
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
        public bool UpdateExtensionNoMember(UpdateExtensionNoMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int errorCode;
            string message;

            if (criteria.IsValid())
            {
                var result = this._repository.AddNewExtensionNoMember(criteria, out errorCode, out message);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Function to delete extension number.
        /// </summary>
        /// <param name="criteria">DeleteExtensionNumber criteria.</param>
        /// <returns>Result of deleted.</returns>
        public bool DeleteExtensionNumber(DeleteExtensionNumberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int errorCode;
            string message;

            if (criteria.IsValid())
            {
                var result = this._repository.DeleteExtensionNumber(criteria, out errorCode, out message);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Function to add external group.
        /// </summary>
        /// <param name="criteria">AddNewExternalGroup Criteria.</param>
        /// <returns>Result of added.</returns>
        public bool AddNewExternalGroup(AddNewExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int errorCode;
            string message;

            if (criteria.IsValid())
            {
                var result = this._repository.AddNewExternalGroup(criteria, out errorCode, out message);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Function to update external group.
        /// </summary>
        /// <param name="criteria">UpdateExternalGroup Criteria.</param>
        /// <returns>Result of updated.</returns>
        public bool UpdateExternalGroup(UpdateExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int errorCode;
            string message;

            if (criteria.IsValid())
            {
                var result = this._repository.UpdateExternalGroup(criteria, out errorCode, out message);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Function to delete external group.
        /// </summary>
        /// <param name="criteria">DeleteExternalGroup Criteria.</param>
        /// <returns>Result of deleted.</returns>
        public bool DeleteExternalGroup(DeleteExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int errorCode;
            string message;

            if (criteria.IsValid())
            {
                var result = this._repository.DeleteExtensionNumber(criteria, out errorCode, out message);
                return result;
            }

            return false;
        }
    }
}
