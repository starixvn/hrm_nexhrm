// -----------------------------------------------------------------------
// <copyright file="DepartmentService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.18">Created.</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// The department service.
    /// </summary>
    public class DepartmentService : BaseService, IDepartmentService
    {
        private IDepartmentRepository _departmentRepository;

        public DepartmentService()
            : this(null)
        {
        }

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        /// <summary>
        /// Get all department.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator department model.</returns>
        public List<DepartmentModel> GetAllDepartment(GetAllDepartmentCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            List<DepartmentModel> departments = null;

            if (criteria.IsValid())
            {
                departments = _departmentRepository.GetAllDepartment(criteria);

                if (departments != null)
                {
                    departments.ForEach(x => x.Children.AddRange(departments.Where(y => y.ParentId == x.DepartmentId)));
                }
            }

            return departments;
        }
    }
}
