namespace Fanex.NexHR.ViewModels
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CreateEmployeeViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
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

        public IList<Shift> Shifts { get; set; }
    }
}