// --------------------------------------------------------------------------
// <copyright file="ExtensionNoRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.14">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interface;

    /// <summary>
    /// ExtensionNo Repository.
    /// </summary>
    public class ExtensionNoRepository : IExtensionNoRepository
    {
        public IList<ExtensionNoModel> GetAllExtensionNo(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var lstEntity = db.Query<ExtensionNoModel>(criteria.GetSpParams()).ToList();
                return lstEntity;
            }
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var lstEntity = db.Query<ExtensionNoModel>(criteria.GetSpParams()).ToList();
                return lstEntity;
            }
        }

        public bool AddNewExtensionNoMember(ICriteria criteria, out int errorCode, out string message)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                db.ExecuteNonQuery(criteria.GetSpParams());
                GetCommandResult(hash, out errorCode, out message);
                return errorCode == 0;
            }
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<ExtensionNoModel>(criteria.GetSpParams());
                return entity;
            }
        }

        /// <summary>
        /// Method update Extension Number member.
        /// </summary>
        /// <param name="criteria">UpdateExtensionNoMember Criteria.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check Result.</returns>
        public bool UpdateExtensionNoMember(ICriteria criteria, out int errorCode, out string message)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                db.ExecuteNonQuery(criteria.GetSpParams());
                GetCommandResult(hash, out errorCode, out message);
                return errorCode == 0;
            }
        }

        /// <summary>
        /// Method Delete ExtensionNumber.
        /// </summary>
        /// <param name="criteria">DeleteExtensionNumber Criteria.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool DeleteExtensionNumber(ICriteria criteria, out int errorCode, out string message)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                db.ExecuteNonQuery(criteria.GetSpParams());
                GetCommandResult(hash, out errorCode, out message);
                return errorCode == 0;
            }
        }

        /// <summary>
        /// Method Add New External Group.
        /// </summary>
        /// <param name="criteria">AddNewExternalGroup Criteria.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool AddNewExternalGroup(ICriteria criteria, out int errorCode, out string message)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                db.ExecuteNonQuery(criteria.GetSpParams());
                GetCommandResult(hash, out errorCode, out message);
                return errorCode == 0;
            }
        }

        /// <summary>
        /// Method Update an External Group.
        /// </summary>
        /// <param name="criteria">UpdateExternalGroup Criteria.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool UpdateExternalGroup(ICriteria criteria, out int errorCode, out string message)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                db.ExecuteNonQuery(criteria.GetSpParams());
                GetCommandResult(hash, out errorCode, out message);
                return errorCode == 0;
            }
        }

        /// <summary>
        /// Method Delete external group.
        /// </summary>
        /// <param name="criteria">DeleteExternalGroup Criteria.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool DeleteExternalGroup(ICriteria criteria, out int errorCode, out string message)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                db.ExecuteNonQuery(criteria.GetSpParams());
                GetCommandResult(hash, out errorCode, out message);
                return errorCode == 0;
            }
        }

        private void GetCommandResult(Hashtable hash, out int errorCode, out string message)
        {
            errorCode = (hash["errcode"] is DBNull) ? 0 : Convert.ToInt32(hash["errcode"]);
            message = (hash["errmsg"] is DBNull) ? string.Empty : Convert.ToString(hash["errmsg"]);
        }
    }
}
