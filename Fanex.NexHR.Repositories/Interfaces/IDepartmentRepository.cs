// -----------------------------------------------------------------------
// <copyright file="IDepartmentRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.18">Created.</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    /// <summary>
    /// The interface of department repository.
    /// </summary>
    public interface IDepartmentRepository
    {
        IEnumerable<DepartmentModel> GetAllDepartment(ICriteria criteria);

        DepartmentModel GetDepartmentById(ICriteria criteria);
    }
}
