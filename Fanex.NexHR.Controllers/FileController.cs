namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Hosting;
    using System.Threading.Tasks;

    public class FileController : BaseController
    {
        private readonly IWebHostEnvironment environment;

        public FileController(
            ISessionService sessionService,
            IWebHostEnvironment environment
        ) : base(sessionService)
        {
            this.environment = environment;
        }

        [HttpPost]
        public async Task<string> UploadFile(List<IFormFile> files, int categoryId, int overRide)
        {
           var attachedFilePath = new StringBuilder();

           if (files == null)
           {
               return attachedFilePath.ToString();
           }

           var dir = $"file\\uploads\\{categoryId}";
           CreateOrOpenDirectory(dir);

           foreach (var file in files)
           {
               string savedFilePath = await SaveFileAsync(overRide, file, dir);
               attachedFilePath.Append("/" + savedFilePath + ";");
           }

            return attachedFilePath.ToString().Remove(0, 1);
        }

        private async Task<string> SaveFileAsync(int overRide, IFormFile file, string dir)
        {
           if (file == null)
           {
               return null;
           }

           var fileName = Path.GetFileName(file.FileName);
           var pathAttach = new StringBuilder($"{dir}/{fileName}");

           fileName = ChangeNameForDuplicatedFile(overRide, pathAttach.ToString(), fileName);

           if (fileName == null)
           {
               return null;
           }

           var path = Path.Combine(Path.Combine(environment.WebRootPath, dir), fileName);

            var stream = new FileStream(path, FileMode.Create);
            await CopyFiles(file, stream);

            return pathAttach.ToString();
        }

        private static async Task CopyFiles(IFormFile file, FileStream stream)
        {
            await file.CopyToAsync(stream);
        }

        private string ChangeNameForDuplicatedFile(int overRide, string pathAttach, string fileName)
        {
           return overRide == 0 && System.IO.File.Exists(Path.Combine(environment.WebRootPath, pathAttach))
               ? $"{Guid.NewGuid().ToString()}-{fileName}"
               : fileName;
        }

        private void CreateOrOpenDirectory(string dir)
        {
           if (!Directory.Exists(Path.Combine(environment.WebRootPath, dir)))
           {
               Directory.CreateDirectory(Path.Combine(environment.WebRootPath, dir));
           }
        }
    }
}
