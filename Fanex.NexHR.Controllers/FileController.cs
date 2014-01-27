// --------------------------------------------------------------------------
// <copyright file="FileController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// File controller.
    /// </summary>
    public class FileController : BaseController
    {
        /// <summary>
        /// Upload file.
        /// </summary>
        /// <param name="file">The upload file.</param>
        /// <param name="categoryID">The category ID.</param>
        /// <param name="overRide">Override or not.</param>
        /// <returns>The return string.</returns>
        [HttpPost]
        public string UploadFile(HttpPostedFileBase file, int categoryID, int overRide)
        {
            string pathAttach = string.Empty;
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var dir = "~/file/uploads/" + categoryID;
                pathAttach = dir + "/" + fileName;

                if (overRide == 0)
                {
                    while (System.IO.File.Exists(Server.MapPath(pathAttach)))
                    {
                        fileName = "(1)" + fileName;
                        pathAttach = dir + "/" + fileName;
                    }
                }

                if (!Directory.Exists(Server.MapPath(dir)))
                {
                    Directory.CreateDirectory(Server.MapPath(dir));
                }

                var path = Path.Combine(Server.MapPath(dir), fileName);

                pathAttach = pathAttach.Substring(2);

                file.SaveAs(path);
            }

            return pathAttach;
        }

        public bool Delete(string fileName)
        {
            System.IO.File.Delete(Server.MapPath("~/" + fileName));
            return true;
        }
    }
}
