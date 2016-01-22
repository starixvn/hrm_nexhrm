// --------------------------------------------------------------------------
// <copyright file="ToeicManagementViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.01.05">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class ToeicManagementViewModel
    {
        public ToeicManagementViewModel()
        {
            Groups = new List<Team>();
            JobTitles = new List<LookupData>();
        }

        #region Filter

        public string Query { get; set; }

        public int? JobTitleId { get; set; }

        public int? GroupId { get; set; }

        public IList<Team> Groups { get; set; }

        public IList<LookupData> JobTitles { get; set; }

        #endregion Filter
    }
}