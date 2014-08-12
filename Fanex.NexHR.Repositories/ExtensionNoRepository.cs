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
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

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

        public ExtensionNoModel GetEmployeeExtensionNoById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<ExtensionNoModel>(criteria.GetSpParams());
                return entity;
            }
        }
    }
}
