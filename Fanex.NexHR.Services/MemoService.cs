namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Core;
    using Criteria.Memo;
    using Interfaces;
    using Models.Memo;
    using Resources;

    public class MemoService : BaseService, IMemoService
    {
        private readonly IEmailService _emailService;
        private const string Email = "Email";

        public MemoService(IEmailService emailService = null)
        {
            _emailService = emailService ?? new EmailService();
        }

        /// <summary>
        /// Get all memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of memo.</returns>
        public IList<Memo> GetAll(GetAllMemoCriteria criteria)
        {
            IList<Memo> entityList = GetData<Memo>(criteria).ToList();
            return entityList;
        }

        /// <summary>
        /// Get memo by ID.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The memo object.</returns>
        public Memo GetById(GetMemoByIdCriteria criteria)
        {
            var memo = GetDataEntity<Memo>(criteria);
            if (memo != null && memo.MemoType == 1)
            {
                string attachFile = memo.Contents;

                memo.AttachFile = attachFile;

                string[] files = memo.AttachFile.Split(';');

                StringBuilder content = new StringBuilder();
                foreach (string file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        string filePath = HttpUtility.HtmlEncode(file);
                        string fileName = HttpUtility.HtmlEncode(file.Split('/')[file.Split('/').Length - 1]);
                        content.Append($"<a href=\'{filePath}\'> {fileName}</a></br>");
                    }
                }
                memo.Contents = content.ToString();
            }

            return memo;
        }

        public void SendMail(string subject, string memoId)
        {
            var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
            var groupSd = ConfigurationManager.AppSettings["EmailSD"];
            var groupGo = ConfigurationManager.AppSettings["EmailGo"];
            var groupNetwork = ConfigurationManager.AppSettings["EmailNetwork"];
            //// send email(s)
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForMemoNotification));

            var body = string.Format(bodyTemplate, subject, link + "memo/detail/" + memoId, DateTime.Now.Year.ToString(), ConfigurationManager.AppSettings["CompanyMailSign"]);
            _emailService.SendEmail(
                ConfigurationManager.AppSettings[Email],
                groupGo,
                string.Empty,
                Messages.MemoSubject + " " + subject,
                body);
            _emailService.SendEmail(
               ConfigurationManager.AppSettings[Email],
               groupSd,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
            _emailService.SendEmail(
               ConfigurationManager.AppSettings[Email],
               groupNetwork,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
        }
    }
}