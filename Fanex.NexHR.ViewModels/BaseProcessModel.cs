// --------------------------------------------------------------------------
// <copyright file="BaseProcessModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NexHR.Models;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class BaseProcessModel
    {
        [Required]
        public long RequestId { get; set; }

        [Display(Name = "Requested by")]
        public string From { get; set; }

        public byte Status { get; set; }

        public Employee Employee { get; set; }
    }
}
