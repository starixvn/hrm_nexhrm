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
        private DateTime? _listeningReadingExamDate;
        private DateTime? _speakingWritingExamDate;

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
                if (!_listeningReadingExamDate.HasValue || _listeningReadingExamDate.Value == DateTime.MinValue)
                {
                    _listeningReadingExamDate = null;
                }

                return _listeningReadingExamDate;
            }

            set
            {
                _listeningReadingExamDate = value;
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
                if (!_speakingWritingExamDate.HasValue || _speakingWritingExamDate.Value == DateTime.MinValue)
                {
                    _speakingWritingExamDate = null;
                }

                return _speakingWritingExamDate;
            }

            set
            {
                _speakingWritingExamDate = value;
            }
        }

        [Range(0, 200)]
        public int? Speaking { get; set; }

        [Range(0, 200)]
        public int? Writing { get; set; }
    }
}
