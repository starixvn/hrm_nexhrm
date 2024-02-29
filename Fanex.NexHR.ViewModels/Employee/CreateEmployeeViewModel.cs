namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Models;

    public class CreateEmployeeViewModel
    {
        [Required]
        public string FullName { get; set; }

        public string Phone { get; set; }

        public string EducationBackground { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        [Required]
        [Display(Name = "English Name")]
        public string NickName { get; set; }

        [Required]
        public string DomainUsername { get; set; }

        [Required]
        public string Code { get; set; }

        public int? MonShift { get; set; }

        public int? TueShift { get; set; }

        public int? WedShift { get; set; }

        public int? ThuShift { get; set; }

        public int? FriShift { get; set; }

        public int? SatShift { get; set; }

        public int? SunShift { get; set; }

        [Required]
        public int TeamId { get; set; }

        public int ProcessBy { get; set; }

        public IList<Team> Teams { get; set; }

        public IEnumerable<Shift> Shifts { get; set; }
    }
}