namespace Fanex.NexHR.Controllers
{
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

            foreach (HttpPostedFileBase file in files)
            {
                if (file.ContentLength <= 0)
                {
                    continue;
                }

                var fileName = Path.GetFileName(file.FileName);
                var dir = $"~/file/uploads/{categoryId}";

                var pathAttach = new StringBuilder($"{dir}/{fileName}");

                if (overRide == 0)
                {
                    while (System.IO.File.Exists(Server.MapPath(pathAttach.ToString())))
                    {
                        fileName = $"(1){fileName}";
                    }
                }

                CreateDirectory(dir);

                if (fileName == null)
                {
                    continue;
                }

                var path = Path.Combine(Server.MapPath(dir), fileName);

                pathAttach = new StringBuilder(pathAttach.ToString().Substring(2));

                if (attachedFilePath.Length > 0)
                {
                    attachedFilePath = pathAttach;
                }
                else
                {
                    attachedFilePath.Append($";/{pathAttach}");
                }

                file.SaveAs(path);
            }

            return attachedFilePath.ToString();
        }

        private void CreateDirectory(string dir)
        {
            if (!Directory.Exists(Server.MapPath(dir)))
            {
                Directory.CreateDirectory(Server.MapPath(dir));
            }
        }
    }
}
