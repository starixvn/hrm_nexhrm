namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    public class MemoService : BaseService, IMemoService
    {
        private readonly IEmailService _emailService;

        public MemoService(IEmailService emailService = null)
        {
            this._emailService = emailService ?? new EmailService();
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
            Memo memo = null;
            memo = GetDataEntity<Memo>(criteria);
            if (memo != null && memo.MemoType == 1)
            {
                memo.AttachFile = memo.Contents;
                memo.Contents =
                    "<a href='"
                    + HttpUtility.HtmlEncode(memo.AttachFile)
                    + "'> "
                    + HttpUtility.HtmlEncode(memo.AttachFile.Split('/')[memo.AttachFile.Split('/').Length - 1])
                    + "</a>";
            }

            return memo;
        }

        public void SendMail(string subject, string memoId)
        {
            var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
            var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
            var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
            var groupNetwork = ConfigurationManager.AppSettings["EmailNetwork"].ToString();
            //// send email(s)
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/MemoNotification.cshtml"));

            var content = new
            {
                Subject = subject,
                Link = link + "memo/detail/" + memoId
            };
            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                groupGo,
                string.Empty,
                Messages.MemoSubject + " " + subject,
                body);
            this._emailService.SendEmail(
               ConfigurationManager.AppSettings["Email"],
               groupSd,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
            this._emailService.SendEmail(
               ConfigurationManager.AppSettings["Email"],
               groupNetwork,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
        }
    }
}
