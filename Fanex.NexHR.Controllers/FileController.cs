namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    public class FileController : BaseController
    {
        [HttpPost]
        public string UploadFile(IEnumerable<HttpPostedFileBase> files, int categoryId, int overRide)
        {
            var attachedFilePath = new StringBuilder();

            if (files == null)
            {
                return attachedFilePath.ToString();
            }

            var dir = $"~/file/uploads/{categoryId}";
            CreateOrOpenDirectory(dir);
            
            foreach (HttpPostedFileBase file in files)
            {
                string savedFilePath = SaveFile(overRide, file, dir);
                attachedFilePath.Append("/" + savedFilePath + ";");
            }

            return attachedFilePath.ToString().Remove(0, 1);
        }

        private string SaveFile(int overRide, HttpPostedFileBase file, string dir)
        {
            if (file.ContentLength <= 0)
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

            const int startIndex = 2;
            var path = Path.Combine(Server.MapPath(dir), fileName);
            file.SaveAs(path);

            return pathAttach.ToString().Substring(startIndex);
        }

        private string ChangeNameForDuplicatedFile(int overRide, string pathAttach, string fileName)
        {
            return overRide == 0 && System.IO.File.Exists(Server.MapPath(pathAttach))
                ? $"{Guid.NewGuid().ToString()}-{fileName}"
                : fileName;
        }

        private void CreateOrOpenDirectory(string dir)
        {
            if (!Directory.Exists(Server.MapPath(dir)))
            {
                Directory.CreateDirectory(Server.MapPath(dir));
            }
        }
    }
}
