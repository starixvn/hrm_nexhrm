namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Parent", ResourceType = typeof(Labels))]
        public int? ParentId { get; set; }

        public int? ManagerId { get; set; }

        [Display(Name = "Manager", ResourceType = typeof(Labels))]
        public string ManagerName { get; set; }
    }
}