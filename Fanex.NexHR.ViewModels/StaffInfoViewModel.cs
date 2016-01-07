// --------------------------------------------------------------------------
// <copyright file="StaffInfoViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.01.07">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class StaffInfoViewModel
    {
        public StaffInfoViewModel()
        {
            Groups = new List<Team>();
        }

        public IList<Team> Groups { get; set; }
    }
}