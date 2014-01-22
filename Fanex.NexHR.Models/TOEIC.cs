// --------------------------------------------------------------------------
// <copyright file="TOEIC.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// TOEIC model.
    /// </summary>
    public class TOEIC
    {
        private DateTime? lrExamDate;
        private DateTime? swExamDate;

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

        public int? LRTotal
        {
            get
            {
                return Listening + Reading;
            }
        }

        [Range(0, 495)]
        public int? OldListening { get; set; }

        [Range(0, 495)]
        public int? OldReading { get; set; }

        [Range(0, 200)]
        public int? OldSpeaking { get; set; }

        [Range(0, 200)]
        public int? OldWriting { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
