// --------------------------------------------------------------------------
// <copyright file="TOEICViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.06">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    public class TOEICViewModel
    {
        private DateTime? lrExamDate;
        private DateTime? swExamDate;

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmpId { get; set; }

        [Display(Name = "Expired Date")]
        public DateTime? InputDate { get; set; }

        [Display(Name = "L/R Exam Date")]
        public DateTime? LRExamDate
        {
            get
            {
                if (!lrExamDate.HasValue || lrExamDate.Value == DateTime.MinValue)
                {
                    lrExamDate = null;
                }

                return lrExamDate;
            }

            set
            {
                lrExamDate = value;
            }
        }

        [Range(0, 495)]
        public int? Listening { get; set; }

        [Range(0, 495)]
        public int? Reading { get; set; }

        [Display(Name = "S/W Exam Date")]
        public DateTime? SWExamDate
        {
            get
            {
                if (!swExamDate.HasValue || swExamDate.Value == DateTime.MinValue)
                {
                    swExamDate = null;
                }

                return swExamDate;
            }

            set
            {
                swExamDate = value;
            }
        }

        [Range(0, 200)]
        public int? Speaking { get; set; }

        [Range(0, 200)]
        public int? Writing { get; set; }
    }
}
