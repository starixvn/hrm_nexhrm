// --------------------------------------------------------------------------
// <copyright file="SearchEmployeeModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    /// <summary>
    /// Search employee model.
    /// </summary>
    public class SearchEmployeeModel
    {
        public string Query { get; set; }

        public int? Team { get; set; }
    }
}
