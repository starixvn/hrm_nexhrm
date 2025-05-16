namespace Fanex.NexHR.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class StaffInfo
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string DomainUsername { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GroupDate { get; set; }

        public string Skype { get; set; }

        public string DepartmentName { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public string JobTitle { get; set; }
    }
}