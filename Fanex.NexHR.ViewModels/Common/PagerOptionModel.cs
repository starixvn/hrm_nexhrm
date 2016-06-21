// --------------------------------------------------------------------------
// <copyright file="PagerOptionModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.01.12">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Common
{
    public class PagerOptionModel
    {
        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalItem { get; set; }
    }
}