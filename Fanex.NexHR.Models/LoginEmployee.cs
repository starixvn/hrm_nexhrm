namespace Fanex.NexHR.Models
{
    public class LoginEmployee
    {
        public int Id { get; set; }

        public string DomainUsername { get; set; }

        public string Password { get; set; }

        public bool IsDeleted { get; set; }

        public bool Enabled { get; set; }
    }
}