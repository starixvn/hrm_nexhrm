namespace Fanex.NexHR.Services
{
    using Core;
    using Criteria.Memo;
    using Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Models.Memo;
    using Resources;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Net;

    public class MemoService : BaseService, IMemoService
    {
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private const string Email = "Email";
        private readonly string CompanyMailSign = ConfigurationManager.AppSettings["CompanyMailSign"];

        public MemoService(IEmailService emailService, IWebHostEnvironment environment)
        {
            this.emailService = emailService;
            this.environment = environment;
        }

        public IList<Memo> GetAll(GetAllMemoCriteria criteria)
        {
            IList<Memo> entityList = GetData<Memo>(criteria).ToList();
            return entityList;
        }

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
                        string filePath = WebUtility.HtmlEncode(file);
                        string fileName = WebUtility.HtmlEncode(file.Split('/')[file.Split('/').Length - 1]);
                        content.Append($"<a href=\'{filePath}\'> {fileName}</a></br>");
                    }
                }
                memo.Contents = content.ToString();
            }

            return memo;
        }

        public void SendMail(string subject, string memoId)
        {
            var groupSd = ConfigurationManager.AppSettings["EmailSD"];
            var groupGo = ConfigurationManager.AppSettings["EmailGo"];
            var groupNetwork = ConfigurationManager.AppSettings["EmailNetwork"];

            var emailTemplatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForMemoNotification);
            var bodyTemplate = Utilities.ReadTextFile(emailTemplatePath);

            var body = string.Format(bodyTemplate, subject, AppSetting.BaseUrl + "memo/detail/" + memoId, CompanyMailSign, ConfigurationManager.AppSettings["CompanyMailSign"]);
            emailService.SendEmail(
                ConfigurationManager.AppSettings[Email],
                groupGo,
                string.Empty,
                Messages.MemoSubject + " " + subject,
                body);
            emailService.SendEmail(
               ConfigurationManager.AppSettings[Email],
               groupSd,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
            emailService.SendEmail(
               ConfigurationManager.AppSettings[Email],
               groupNetwork,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
        }
    }
}