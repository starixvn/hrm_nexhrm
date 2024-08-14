namespace Fanex.NexHR.Models.Bio
{
    public class EmployeeFingerprintAndCardSummary
    {
        public int EmpId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string TeamCode { get; set; }

        public int BioUserId { get; set; }

        public int TotalFingerprint { get; set; }

        public string CardNo { get; set; }

        public bool IsFingerprintWarning { get; set; }

        public bool IsCardWarning { get; set; }

        public bool IsWarning
        {
            get
            {
                return IsFingerprintWarning || IsCardWarning;
            }
        }
    }
}