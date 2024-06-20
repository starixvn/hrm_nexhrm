namespace Fanex.NexHR.Controllers
{
    using System.IO;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class AvatarController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly IEmployeeService employeeService;
        private readonly AppSetting appSetting;

        public AvatarController(IWebHostEnvironment environment, IEmployeeService employeeService, AppSetting appSetting)
        {
            this.environment = environment;
            this.employeeService = employeeService;
            this.appSetting = appSetting;
        }

        [AllowAnonymous]
        [Route("Avatars/{id}")]
        public FileResult Avatar(string id)
        {
            var fileName = string.IsNullOrWhiteSpace(id) ? "avatar.jpg" : (id);

            var paths = new[] { environment.WebRootPath, "avatars", fileName };
            var avaPath = Path.Combine(paths);

            if (System.IO.File.Exists(avaPath))
            {
                fileName = "/avatars/" + fileName;
            }

            return File(fileName, "image/jpeg");
        }

        [AllowAnonymous]
        [Route("avatar/{imgNickName}")]
        public async Task<FileResult> GetAvatar(string imgNickName)
        {
            string fileName = "/avatars/avatar.jpg";
            if (!string.IsNullOrWhiteSpace(imgNickName))
            {
                string nickName = imgNickName.EndsWith(".jpg") ? imgNickName[..imgNickName.LastIndexOf(".")] : imgNickName;
                string domainName = $"{appSetting.DomainName}{nickName}";
                var employee = await employeeService.GetEmployeeByDomainName(new GetEmployeeByDomainNameCriteria { DomainUserName = domainName });
                if (employee is not null)
                {
                    var paths = new[] { environment.WebRootPath, "avatars", employee.Code + ".jpg" };
                    var avaPath = Path.Combine(paths);
                    if (System.IO.File.Exists(avaPath))
                    {
                        fileName = $"/avatars/{employee.Code}.jpg";
                    }
                }
            }
            return File(fileName, "image/jpeg");
        }
    }
}