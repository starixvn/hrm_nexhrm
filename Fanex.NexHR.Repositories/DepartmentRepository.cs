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
        /// <returns>The enumerator department model.</returns>
        public IEnumerable<DepartmentModel> GetAllDepartment(ICriteria criteria)
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
                    criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Get department by Id.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The department.</returns>
        public DepartmentModel GetDepartmentById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<DepartmentModel, Employee, Employee, Employee, DepartmentModel>(
                    (dep, cre, mag, modi) =>
                    {
                        dep.Creator = cre;
                        dep.Manager = mag;
                        dep.Modifier = modi;
                        return dep;
                    },
                    "CreatedBy, ManagerId, ModifedBy",
                    criteria.GetSpParams());
            }
        }
    }
}
