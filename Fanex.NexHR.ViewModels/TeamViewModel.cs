namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Models;
    using Resources;

    public class TeamViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Code", ResourceType = typeof(Labels))]
        public string TeamCode { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(Labels))]
        public string TeamName { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Parent Team")]
        public int? ParentId { get; set; }

        public IList<Department> Departments { get; set; }

        public IList<Team> Teams { get; set; }
    }
}