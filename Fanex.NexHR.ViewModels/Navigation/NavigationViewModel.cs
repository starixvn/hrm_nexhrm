namespace Fanex.NexHR.ViewModels.Navigation
{
    using System.Collections.Generic;
    using System.Globalization;

    public class NavigationViewModel
    {
        private string _controller;
        private string _action;
        private string _title;

        public NavigationViewModel()
        {
        }

        //public NavigationViewModel(SiteMapNode sitemapNode)
        //{
        //    if (sitemapNode != null)
        //    {
        //        Controller = sitemapNode["Controller"];
        //        Action = sitemapNode["Action"];
        //        Title = sitemapNode.Title;
        //        Icon = sitemapNode["Icon"];
        //        HoverController = sitemapNode["HoverController"];
        //        HoverAction = sitemapNode["HoverAction"];
        //        HoverIcon = sitemapNode["HoverIcon"];
        //    }
        //}

        public string Icon { get; set; }

        public string Controller
        {
            get
            {
                return _controller.ToLower(CultureInfo.InvariantCulture);
            }

            set
            {
                _controller = value ?? string.Empty;
            }
        }

        public string Action
        {
            get
            {
                return _action.ToLower(CultureInfo.InvariantCulture);
            }

            set
            {
                _action = value ?? string.Empty;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value ?? string.Empty;
            }
        }

        public IDictionary<string, object> HtmlAttributes { get; set; }

        public List<NavigationViewModel> Childs { get; set; }

        public string Id => $"{Controller}_{Action}";

        public string HoverController { get; set; }

        public string HoverAction { get; set; }

        public string HoverIcon { get; set; }
    }
}
