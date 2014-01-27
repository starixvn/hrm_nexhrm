// --------------------------------------------------------------------------
// <copyright file="ChangeShiftViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Change shift view model.
    /// </summary>
    public class ChangeShiftViewModel : BaseRequestModel, IValidatableObject
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime ForDate { get; set; }

        [Required]
        public int FromShift { get; set; }

        public string FromShiftName { get; set; }

        [Required]
        public int ToShift { get; set; }

        public string ToShiftName { get; set; }

        public bool IsPlanned { get; set; }

        /// <summary>
        /// Valiate context.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <returns>List of validation result.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (FromShift == 0)
            {
                var validation = new ValidationResult("Invalid FromShift value", new[] { "FromShift" });

                result.Add(validation);
            }

            if (ToShift == 0)
            {
                var validation = new ValidationResult("Invalid ToShift value", new[] { "ToShift" });

                result.Add(validation);
            }

            if (ToShift == FromShift)
            {
                var validation = new ValidationResult(Messages.ToShiftMustDifferentFromFromShift, new[] { "ToShift" });

                result.Add(validation);
            }

            return result;
        }
    }
}
