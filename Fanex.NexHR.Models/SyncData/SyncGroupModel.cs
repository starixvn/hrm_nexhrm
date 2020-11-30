namespace Fanex.NexHR.Models.SyncData
{
    using System;

    public class SyncGroupModel
    {
        public int GroupId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? DepartmentId { get; set; }

        public int? LeaderId { get; set; }

        public int? AssistantLeaderId { get; set; }

        public int Rank { get; set; }

        public bool IsDeleted { get; set; }

        public int? Left { get; set; }

        public int? Right { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}