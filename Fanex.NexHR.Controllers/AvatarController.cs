namespace Fanex.NexHR.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;

    public class AvatarController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IWebHostEnvironment environment;

        public AvatarController(IWebHostEnvironment environment)
        {
            this.environment = environment;
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
    }
}