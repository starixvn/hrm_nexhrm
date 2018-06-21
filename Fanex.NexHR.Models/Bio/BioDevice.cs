namespace Fanex.NexHR.Models.Bio
{
    public class BioDevice
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public int? DisplayId { get; set; }

        public string DisplayName { get; set; }

        public int? StatusId { get; set; }

        public string Status { get; set; }

        public bool Active { get; set; }
    }
}