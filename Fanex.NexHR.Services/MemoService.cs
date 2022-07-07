namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Core;
    using Criteria.Memo;
    using Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Models.Memo;
    using Resources;

    public class MemoService : BaseService, IMemoService
    {
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly AppSetting appSettings;

        public MemoService(IEmailService emailService, IWebHostEnvironment environment, AppSetting appSettings)
        {
            this.emailService = emailService;
            this.environment = environment;
            this.appSettings = appSettings;
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
            var groupSd = appSettings.EmailSD;
            var groupGo = appSettings.EmailGo;
            var groupNetwork = appSettings.EmailNetwork;

            var emailTemplatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForMemoNotification);
            var bodyTemplate = Utilities.ReadTextFile(emailTemplatePath);
            string companyMailSign = appSettings.CompanyMailSign;

            var body = string.Format(bodyTemplate, subject, appSettings.BaseUrl + "memo/detail/" + memoId, companyMailSign, appSettings.CompanyMailSign);
            emailService.SendEmail(
                appSettings.Email,
                groupGo,
                string.Empty,
                Messages.MemoSubject + " " + subject,
                body);
            emailService.SendEmail(
               appSettings.Email,
               groupSd,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
            emailService.SendEmail(
               appSettings.Email,
               groupNetwork,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
        }
    }
}