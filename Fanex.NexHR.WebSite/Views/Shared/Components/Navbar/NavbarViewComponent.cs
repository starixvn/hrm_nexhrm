namespace Fanex.NexHR.WebSite.Views.Shared.Components.Navbar
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Navigation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Newtonsoft.Json;

    public class NavbarViewComponent : ViewComponent
    {
        private readonly IActionContextAccessor actionContextAccessor;
        private readonly ISessionService sessionService;
        private readonly IWebHostEnvironment hostEnvironment;

        public NavbarViewComponent(IActionContextAccessor actionContextAccessor,
            ISessionService sessionService,
            IWebHostEnvironment hostEnvironment)
        {
            this.actionContextAccessor = actionContextAccessor;
            this.sessionService = sessionService;
            this.hostEnvironment = hostEnvironment;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var routeData = actionContextAccessor.ActionContext?.RouteData;
            ViewBag.Controller = routeData?.Values["controller"]?.ToString()?.ToLowerInvariant();
            ViewBag.Action = routeData?.Values["action"]?.ToString()?.ToLowerInvariant();
            ViewBag.NavigationItems = await GetNagivationData();

            return View();
        }

        public async Task<IList<NavigationViewModel>> GetNagivationData()
        {
            var roles = sessionService.CurrentEmployee.RoleList;
            List<Role> roleList = new List<Role>();
            foreach (var role in roles)
            {
                roleList.Add((Role)role);
            }

            if (!roleList.Exists(r => r == Role.AssistantLeader) && roleList.Exists(r => r == Role.Leader))
            {
                roleList.Add(Role.AssistantLeader);
            }

            if (!roleList.Exists(r => r == Role.Member))
            {
                roleList.Add(Role.Member);
            }

            var navigationItemList = await GetNavigation(roleList);

            return navigationItemList;
        }

        private async Task<IList<NavigationViewModel>> GetNavigation(IList<Role> roles)
        {
            var menus = await ReadNavigationJson();

            var menuItems = menus.Where(m => m.ParentId == null && (roles.Any(r => r == m.Roles) || roles.Any(r => r == Role.SA)));
            foreach (var menuItem in menuItems)
            {
                var childs = menus.Where(m => m.ParentId == menuItem.Id && (roles.Any(r => r == m.Roles) || roles.Any(r => r == Role.SA)));
                menuItem.Childs = childs.ToList();
            }
            return menuItems.ToList();
        }

        private async Task<IList<NavigationViewModel>> ReadNavigationJson()
        {
            var path = Path.Combine(hostEnvironment.ContentRootPath, "Navigation.json");
            var jsonString = await File.ReadAllTextAsync(path);

            var menus = new List<NavigationViewModel>();

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                menus = JsonConvert.DeserializeObject<List<NavigationViewModel>>(jsonString) ?? new List<NavigationViewModel>();
            }

            return menus;
        }
    }
}