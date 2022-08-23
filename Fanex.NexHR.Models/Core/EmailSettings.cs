namespace Fanex.NexHR.Models.Core
{
    public class EmailSettings
    {
        public bool IsDisabledSendMail { get; set; }
        public string CompanyMailSign { get; set; }
        public string Email { get; set; }
        public string EmailSD { get; set; }
        public string EmailGo { get; set; }
        public string EmailNetwork { get; set; }
        public bool AllowSendMailReminderBirthday { get; set; }
        public string AppMailAddress { get; set; }
        public string AppMailId { get; set; }
    }
}