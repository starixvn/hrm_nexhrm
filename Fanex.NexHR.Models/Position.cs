namespace Fanex.NexHR.Models
{
    public class Position
    {
        public int PositionId { get; set; }

        public string PositionName { get; set; }

        public string PositionCode { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
