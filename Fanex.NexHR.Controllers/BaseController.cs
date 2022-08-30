namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [HrmAuthorize]
    public class BaseController : Controller
    {
        private readonly ISessionService sessionService;

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public Employee CurrentUser
        {
            get
            {
                return sessionService.CurrentEmployee;
            }
        }

        public EmployeeSetting CurrentUserSetting
        {
            get
            {
                return sessionService.CurrentUserSetting;
            }
        }
    }
}