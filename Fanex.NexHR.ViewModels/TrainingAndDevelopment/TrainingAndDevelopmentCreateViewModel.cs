namespace Fanex.NexHR.ViewModels.TrainingAndDevelopment
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Resources;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TrainingAndDevelopmentCreateViewModel
    {
        [Required]
        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        [Display(Name = "ConductedBy", ResourceType = typeof(Labels))]
        [MaxLength(Constants.TrainingConductedByLength)]
        public string ConductedBy { get; set; }

        [Required]
        [Display(Name = "Type", ResourceType = typeof(Labels))]
        public int? TypeId { get; set; }

        [Required]
        [Display(Name = "Publish", ResourceType = typeof(Labels))]
        public string Publish { get; set; }

        public string TypeName { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(Labels))]
        [MaxLength(Constants.TrainingNameLength)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Organization", ResourceType = typeof(Labels))]
        [MaxLength(Constants.TrainingOrganizationLength)]
        public string Organization { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [MaxLength(Constants.TrainingDescriptionLength)]
        public string Description { get; set; }

        public DateTime? FromDate { get; set; }

        [TrainingDateValidator]
        public DateTime? ToDate { get; set; }

         public string Certificate { get; set; }

        public IFormFile FileUpload { get; set; }
    }
}
