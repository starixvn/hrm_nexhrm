namespace Fanex.NexHR.ViewModels.SkypeAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Resources;

    public class SkypeAccountViewModel
    {
        public int EmployeeId { get; set; }

        [Display(Name = nameof(Labels.FullName), ResourceType = typeof(Labels))]
        public string FullName { get; set; }

        [Display(Name = nameof(Labels.NickName), ResourceType = typeof(Labels))]
        public string NickName { get; set; }

        [Required]
        [Display(Name = nameof(Labels.SkypeName), ResourceType = typeof(Labels))]
        public string ConversationId { get; set; }

        public IEnumerable<SelectListItem> SkypeUserSuggestion { get; set; }
    }
}