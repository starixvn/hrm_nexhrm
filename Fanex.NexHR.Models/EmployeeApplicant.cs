namespace Fanex.NexHR.Models
{
    public class EmployeeApplicant
    {
        public int Id { get; set; }

        public string ExpectedSalary { get; set; }

        public string OfferedSalary { get; set; }

        public string FinalComment { get; set; }

        public byte[] FileContent { get; set; }


        public string FileContentType { get; set; }

        public string FileExtension { get; set; }

    }
}
