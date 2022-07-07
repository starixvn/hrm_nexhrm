namespace Fanex.NexHR.ViewModels.Navigation
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;

    public class NavigationViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Icon { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Title { get; set; }
        public Role Roles { get; set; }
        public IDictionary<string, object> HtmlAttributes { get; set; }
        public List<NavigationViewModel> Childs { get; set; }
        public string ElementId => $"{Controller}_{Action}";
    }
}