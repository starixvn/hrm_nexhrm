// -----------------------------------------------------------------------
// <copyright file="DepartmentModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.18">Created.</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The department model.
    /// </summary>
    public class DepartmentModel
    {
        public DepartmentModel()
        {
            Children = new List<DepartmentModel>();
        }

        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDisabled { get; set; }

        public int DepartmentLevel { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? DisabledDate { get; set; }

        public Employee Creator { get; set; }

        public Employee Manager { get; set; }

        public Employee Modifier { get; set; }

        public List<DepartmentModel> Children { get; set; }
    }
}
