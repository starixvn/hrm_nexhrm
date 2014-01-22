// --------------------------------------------------------------------------
// <copyright file="BaseAnnualLeaveBalanceViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Base annual leave balance view model.
    /// </summary>
    public class BaseAnnualLeaveBalanceViewModel : IValidatableObject
    {
        [Required]
        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Affect from")]
        public DateTime AffectedFrom { get; set; }

        public decimal DeductDays
        {
            get
            {
                return this.TotalDays < 0 ? -this.TotalDays : 0;
            }
        }

        public decimal CreditDays
        {
            get
            {
                return this.TotalDays > 0 ? this.TotalDays : 0;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (this.TotalDays == 0 || this.TotalDays % (decimal)0.5 != 0)
            {
                var validation = new ValidationResult(Messages.InvalidInputData, new[] { "TotalDays" });

                result.Add(validation);
            }

            return result;
        }
    }
}
