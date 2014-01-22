// --------------------------------------------------------------------------
// <copyright file="LoginModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Login model.
    /// </summary>
    public class LoginModel
    {
        [Required]
        [Display(Name = "Domain username")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
