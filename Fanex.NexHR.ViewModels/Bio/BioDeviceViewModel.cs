namespace Fanex.NexHR.ViewModels.Bio
{
    using Fanex.NexHR.Resources;

    public class BioDeviceViewModel
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public int? DisplayId { get; set; }

        public string DisplayName { get; set; }

        public int? StatusId { get; set; }

        public string Status { get; set; }

        public bool Active { get; set; }

        public string ActiveLabel
        {
            get
            {
                return Active ? Labels.Yes : Labels.No;
            }
        }
    }
}