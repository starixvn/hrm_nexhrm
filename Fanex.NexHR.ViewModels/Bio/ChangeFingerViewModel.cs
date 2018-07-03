namespace Fanex.NexHR.ViewModels.Bio
{
    using Fanex.NexHR.Core.Enums;

    public class ChangeFingerViewModel
    {
        public int EmployeeId { get; set; }

        public int BioUserId { get; set; }

        public int Index { get; set; }

        public int TemplateIndex { get; set; }

        public FingerprintScannerStatus Status { get; set; }
    }
}