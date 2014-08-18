// -----------------------------------------------------------------------
// <copyright file="DepartmentRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.18">Created.</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// The department repository.
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        /// <summary>
        /// Get all department.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list department model.</returns>
        public List<DepartmentModel> GetAllDepartment(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DepartmentModel, Employee, Employee, Employee, DepartmentModel>(
                    (de, cre, man, modi) =>
                    {
                        de.Creator = cre;
                        de.Manager = man;
                        de.Modifier = modi;
                        return de;
                    },
                    "CreatedBy, ManagerId, ModifiedBy",
                    criteria.GetSpParams()).ToList();
            }
        }
    }
}
