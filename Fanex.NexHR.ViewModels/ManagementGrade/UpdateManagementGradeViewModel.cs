using System;

namespace Fanex.NexHR.ViewModels.ManagementGrade
{
    public class UpdateManagementGradeViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int OldManagementGradeId { get; set; }

        public int ManagementGradeId { get; set; }

        public DateTime ManagementEffectiveDate { get; set; }

        public string Note { get; set; }

        public bool IsPromotion { get; set; }
    }
}
