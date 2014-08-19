// -----------------------------------------------------------------------
// <copyright file="IDepartmentService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.18">Created.</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;

    /// <summary>
    /// The interface of department service.
    /// </summary>
    public interface IDepartmentService
    {
        IEnumerable<DepartmentModel> GetAllDepartment(GetAllDepartmentCriteria criteria);

        DepartmentModel GetDepartmentById(GetDepartmentByIdCriteria criteria);

        CommandResult InsertDepartment(InsertDepartmentCriteria criteria);

        CommandResult UpdateDepartment(UpdateDepartmentCriteria criteria);

        CommandResult DeleteDepartment(DeleteDepartmentCriteria criteria);
    }
}
