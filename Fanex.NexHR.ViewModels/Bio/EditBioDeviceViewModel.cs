namespace Fanex.NexHR.ViewModels.Bio
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Resources;

    public class EditBioDeviceViewModel
    {
        public int DeviceId { get; set; }

        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.Status))]
        public string StatusId { get; set; }

        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.DisplayName))]
        public string DisplayName { get; set; }

        public IEnumerable<SelectListItem> BioDeviceStatuses { get; set; }
    }
}