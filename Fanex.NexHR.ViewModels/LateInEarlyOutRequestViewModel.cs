// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutRequestViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class LateInEarlyOutRequestViewModel : BaseRequestModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime ForDate { get; set; }

        [Required]
        [Range(1, 60)]
        public int Minute { get; set; }

        [Required]
        public short Type { get; set; }
    }
}
