namespace Fanex.NexHR.Models.SyncData
{
    using System;

    public class SyncEmployeeModel
    {
        public int SourceEmpId { get; set; }

        public string DomainUsername { get; set; }

        public string Code { get; set; }

        public string Nickname { get; set; }

        public string FullName { get; set; }

        public int JobTitleId { get; set; }

        public int? GroupId { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSpecialUser { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTime? JoinedDate { get; set; }

        public DateTime? ConfirmedDate { get; set; }

        public DateTime? ResignedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}