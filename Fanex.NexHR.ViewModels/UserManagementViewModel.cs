// --------------------------------------------------------------------------
// <copyright file="UserManagementViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.01.04">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class UserManagementViewModel
    {
        public UserManagementViewModel()
        {
            Groups = new List<Team>();
            JobTitles = new List<LookupData>();
        }

        public string JobTitleId { get; set; }

        public IList<Team> Groups { get; set; }

        public IList<LookupData> JobTitles { get; set; }
    }
}