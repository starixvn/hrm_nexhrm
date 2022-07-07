namespace Fanex.NexHR.WebSite2.Views.Shared.Components.Navbar
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Navigation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;

    public class NavbarViewComponent : ViewComponent
    {
        private readonly IActionContextAccessor actionContextAccessor;
        private readonly ISessionService sessionService;
        private readonly IConfiguration configuration;

        public NavbarViewComponent(IActionContextAccessor actionContextAccessor,
            ISessionService sessionService,
            IConfiguration configuration)
        {
            this.actionContextAccessor = actionContextAccessor;
            this.sessionService = sessionService;
            this.configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var routeData = actionContextAccessor.ActionContext?.RouteData;
            ViewBag.Controller = routeData?.Values["controller"]?.ToString();
            ViewBag.Action = routeData?.Values["action"]?.ToString();
            ViewBag.NavigationItems = GetNagivationData();
            await Task.CompletedTask;
            return View();
        }

        public IList<NavigationViewModel> GetNagivationData()
        {
            var roles = sessionService.CurrentEmployee.RoleList;
            List<Role> roleList = new List<Role>();
            foreach (var role in roles)
            {
                roleList.Add((Role)role);
            }

            if (!roleList.Any(r => r == Role.Member))
            {
                roleList.Add(Role.Member);
            }

            var navigationItemList = GetNavigation(roleList);

            return navigationItemList;
        }

        private IList<NavigationViewModel> GetNavigation(IList<Role> roles)
        {
            var menus = configuration.GetSection("Navigation").Get<List<NavigationViewModel>>();

            var menuItems = menus.Where(m => m.ParentId == null && (roles.Any(r => r == m.Roles) || roles.Any(r => r == Role.SA)));
            foreach (var menuItem in menuItems)
            {
                var childs = menus.Where(m => m.ParentId == menuItem.Id && (roles.Any(r => r == m.Roles) || roles.Any(r => r == Role.SA)));
                menuItem.Childs = childs.ToList();
            }
            return menuItems.ToList();
        }
    }
}