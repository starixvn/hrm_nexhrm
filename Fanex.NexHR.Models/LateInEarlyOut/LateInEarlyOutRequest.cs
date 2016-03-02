namespace Fanex.NexHR.Models.LateInEarlyOut
{
    using System;

    /// <summary>
    /// Late in early out request.
    /// </summary>
    public class LateInEarlyOutRequest
    {
        public int Id { get; set; }

        public DateTime FromDate { get; set; }

        public int LateInMins { get; set; }

        public int EarlyOutMins { get; set; }

        public DateTime CreatedDate { get; set; }

        public byte Status { get; set; }

        public string ProcessedBy { get; set; }

        public string FullNameProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }
    }
}
