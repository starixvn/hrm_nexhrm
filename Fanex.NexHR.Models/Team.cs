namespace Fanex.NexHR.Models
{
    public class Team : EntityBase<int>
    {
        public int TeamId { get; set; }

        public int DepartmentId { get; set; }

        public int ParentId { get; set; }

        public int Rank { get; set; }

        public int NoOfMembers { get; set; }
    }
}
