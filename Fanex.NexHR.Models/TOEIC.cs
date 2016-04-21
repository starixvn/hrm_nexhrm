namespace Fanex.NexHR.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// TOEIC model.
    /// </summary>
    public class Toeic
    {
        private DateTime? _listeningReadingExamDate;
        private DateTime? _speakingWritingExamDate;

        public int EmpId { get; set; }

        [Display(Name = "Expired Date")]
        public DateTime? InputDate { get; set; }

        [Display(Name = "L/R Exam Date")]
        public DateTime? LRExamDate
        {
            get
            {
                if (!_listeningReadingExamDate.HasValue || _listeningReadingExamDate.Value == DateTime.MinValue 
                    || _listeningReadingExamDate.Value.Year == 1900)
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

        public bool SendMail { get; set; }
    }
}
