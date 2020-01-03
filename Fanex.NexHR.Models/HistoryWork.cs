namespace Fanex.NexHR.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HistoryWork : EntityBase<int>
    {
        public int EmpId { get; set; }

        [DataType(DataType.Date)]
        public DateTime AffectedDate { get; set; }

        public int ModifiedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime ModifiedDate { get; set; }

        public string JobtitleName { get; set; }

        public string GroupName { get; set; }

        public Employee Employee { get; set; }

        public string NickName { get; set; }
    }
}
