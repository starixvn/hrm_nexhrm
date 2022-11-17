namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Criteria.Memo;
    using Fanex.NexHR.Models.Core;
    using Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Models.Memo;
    using Resources;

    public class MemoService : IMemoService
    {
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly IBaseService baseService;
        private readonly AppSetting appSettings;
        private readonly EmailSettings emailSettings;

        public MemoService(IEmailService emailService, IWebHostEnvironment environment, IBaseService baseService, AppSetting appSettings, EmailSettings emailSettings)
        {
            this.emailService = emailService;
            this.environment = environment;
            this.baseService = baseService;
            this.appSettings = appSettings;
            this.emailSettings = emailSettings;
        }

        public async Task<IEnumerable<Memo>> GetAll(GetAllMemoCriteria criteria)
        {
            IEnumerable<Memo> entityList = await baseService.GetDataAsync<Memo>(criteria);
            return entityList.ToList();
        }

        public async Task<Memo> GetById(GetMemoByIdCriteria criteria)
        {
            var memo = await baseService.GetDataEntityAsync<Memo>(criteria);
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
            var groupSd = emailSettings.EmailSD;
            var groupGo = emailSettings.EmailGo;
            var groupNetwork = emailSettings.EmailNetwork;

            var emailTemplatePath = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForMemoNotification);
            var bodyTemplate = Utilities.ReadTextFile(emailTemplatePath);
            string companyMailSign = emailSettings.CompanyMailSign;

            var body = string.Format(bodyTemplate, subject,$"{appSettings.BaseUrl}/memo/detail/{memoId}", companyMailSign);
            emailService.SendEmail(
                emailSettings.Email,
                groupGo,
                string.Empty,
                Messages.MemoSubject + " " + subject,
                body);
            emailService.SendEmail(
               emailSettings.Email,
               groupSd,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
            emailService.SendEmail(
               emailSettings.Email,
               groupNetwork,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
        }
    }
}