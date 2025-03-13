using System;

namespace Fanex.NexHR.ViewModels.JobGrade
{
    public class UpdateJobGradeViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int JobGradeId { get; set; }

        public int OldJobGradeId { get; set; }

        public bool IsPromotion { get; set; }

        public DateTime EffectiveDate{ get; set; }

        public string Note { get; set; }
    }
}
