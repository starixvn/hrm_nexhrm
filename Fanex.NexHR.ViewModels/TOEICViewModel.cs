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

    /// <summary>
    /// TOEIC view model.
    /// </summary>
    public class TOEICViewModel
    {
        private DateTime? _LRExamDate;
        private DateTime? _SWExamDate;

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
                if (!_LRExamDate.HasValue || _LRExamDate.Value == DateTime.MinValue)
                {
                    _LRExamDate = null;
                }

                return _LRExamDate;
            }

            set
            {
                _LRExamDate = value;
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
                if (!_SWExamDate.HasValue || _SWExamDate.Value == DateTime.MinValue)
                {
                    _SWExamDate = null;
                }

                return _SWExamDate;
            }

            set
            {
                _SWExamDate = value;
            }
        }

        [Range(0, 200)]
        public int? Speaking { get; set; }

        [Range(0, 200)]
        public int? Writing { get; set; }
    }
}
