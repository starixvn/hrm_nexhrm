// --------------------------------------------------------------------------
// <copyright file="PagerOptionModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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

        public int TotalPage { get; set; }
    }
}