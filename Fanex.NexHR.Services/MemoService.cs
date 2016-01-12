// --------------------------------------------------------------------------
// <copyright file="MemoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

//namespace Fanex.NexHR.Repositories
namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Memo service.
    /// </summary>
    public class MemoService : IMemoService
    {
        private readonly IMemoRepository _memoRepository;
        private readonly IEmailService _emailService;

        public MemoService(IMemoRepository memoRepository = null, IEmailService emailService = null)
        {
            this._memoRepository = memoRepository ?? new MemoRepository();
            this._emailService = emailService ?? new EmailService();
        }

        /// <summary>
        /// Get all memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of memo.</returns>
        public IList<Memo> GetAll(GetAllMemoCriteria criteria)
        {
            IList<Memo> entityList = null;
            if (criteria.IsValid())
            {
                entityList = this._memoRepository.GetAll(criteria);
            }

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
            if (criteria.IsValid())
            {
                memo = this._memoRepository.GetById(criteria);
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
            }

            return memo;
        }

        /// <summary>
        /// Delete memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public bool Delete(DeleteMemoCriteria criteria)
        {
            bool result = false;
            if (criteria.IsValid())
            {
                result = this._memoRepository.Delete(criteria);
            }

            return result;
        }

        /// <summary>
        /// Insert memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public int Insert(InsertMemoCriteria criteria)
        {
            int result = 1;
            if (criteria.IsValid())
            {
                result = this._memoRepository.Insert(criteria);
                var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
                if (result != -1 && criteria.SendMail)
                {
                    var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
                    var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
                    var groupNetwork = ConfigurationManager.AppSettings["EmailNetwork"].ToString();
                    //// send email(s)
                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/MemoNotification.cshtml"));

                    var content = new
                    {
                        Subject = criteria.Subject,
                        Link = link + "memo/detail/" + result.ToString()
                    };
                    var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        groupGo,
                        string.Empty,
                        Messages.MemoSubject + " " + criteria.Subject,
                        body);
                    this._emailService.SendEmail(
                       ConfigurationManager.AppSettings["Email"],
                       groupSd,
                       string.Empty,
                       Messages.MemoSubject + " " + criteria.Subject,
                       body);
                    this._emailService.SendEmail(
                       ConfigurationManager.AppSettings["Email"],
                       groupNetwork,
                       string.Empty,
                       Messages.MemoSubject + " " + criteria.Subject,
                       body);
                }
            }

            return result;
        }

        /// <summary>
        /// Update memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public bool Update(UpdateMemoCriteria criteria)
        {
            bool result = false;
            if (criteria.IsValid())
            {
                result = this._memoRepository.Update(criteria);
                var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;

                if (result && criteria.SendMail)
                {
                    var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
                    var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
                    //// send email(s)
                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/EditMemoNotification.cshtml"));

                    var content = new
                    {
                        Subject = criteria.Subject,
                        Link = link + "memo/detail/" + criteria.Id.ToString()
                    };
                    var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        groupGo,
                        string.Empty,
                        Resources.Messages.MemoSubject + " " + criteria.Subject,
                        body);
                    this._emailService.SendEmail(
                       ConfigurationManager.AppSettings["Email"],
                       groupSd,
                       string.Empty,
                       Resources.Messages.MemoSubject + " " + criteria.Subject,
                       body);
                }
            }

            return result;
        }
    }
}
