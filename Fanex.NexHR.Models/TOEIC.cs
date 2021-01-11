namespace Fanex.NexHR.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    public class Toeic
    {
        private DateTime? _listeningReadingExamDate;
        private DateTime? _speakingWritingExamDate;
        private const int year = 1900;
        private const int maxRangeRL = 495;
        private const int maxRangeSW = 200;
        private const int maxRangeTotal = 990;
        private const int rangeNotRemind = 200;

        public int EmpId { get; set; }

        [Required]
        public string Employee { get; set; }

        [Display(Name = "Expired Date")]
        public DateTime? InputDate { get; set; }

        [Display(Name = "L/R Exam Date")]
        public DateTime? LRExamDate
        {
            get
            {
                if (!_listeningReadingExamDate.HasValue || _listeningReadingExamDate.Value == DateTime.MinValue
                    || _listeningReadingExamDate.Value.Year == year)
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

        [Range(0, maxRangeRL)]
        public int? Listening { get; set; }

        [Range(0, maxRangeRL)]
        public int? Reading { get; set; }

        [ReadOnly(true)]
        public int? Total { get { return Listening + Reading; } }

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

        [Range(0, maxRangeSW)]
        public int? Speaking { get; set; }

        [Range(0, maxRangeSW)]
        public int? Writing { get; set; }

        public int? LRTotal => Listening + Reading;

        [Range(0, maxRangeRL)]
        public int? OldListening { get; set; }

        [Range(0, maxRangeRL)]
        public int? OldReading { get; set; }

        [ReadOnly(true)]
        public int? OldTotal { get { return OldListening + OldReading; } }

        [Range(0, maxRangeSW)]
        public int? OldSpeaking { get; set; }

        [Range(0, maxRangeSW)]
        public int? OldWriting { get; set; }

        [Range(0, maxRangeTotal)]
        public int? MinimumScore { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool SendMail { get; set; }

        public bool IsWarningExpiredDate
        {
            get
            {
                if (MinimumScore.HasValue && InputDate.HasValue)
                {
                    return (!LRTotal.HasValue || (MinimumScore + rangeNotRemind) > LRTotal.Value) 
                        && (InputDate.Value < DateTime.Now);
                }

                return false;
            }
        }

        public bool IsWarningMinimumScore
        {
            get
            {
                if (MinimumScore.HasValue && InputDate.HasValue && LRTotal.HasValue)
                {
                    return MinimumScore.Value > LRTotal.Value;
                }

                return false;
            }
        }
    }
}