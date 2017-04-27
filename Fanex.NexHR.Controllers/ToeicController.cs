namespace Fanex.NexHR.Controllers
{
    using Core;
    using Criteria.Employee;
    using Criteria.Toeic;
    using Models;
    using Resources;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels.Toeic;

    public class ToeicController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmailService _emailService;
        private readonly IJobTitleService _jobTitleService;
        private readonly ITeamService _teamService;
        private readonly IToeicService _toeicService;
        private readonly ICommandService _commandService;

        public ToeicController()
            : this(null, null, null, null, null, null)
        {
        }

        public ToeicController(
            IEmployeeService employeeService,
            IEmailService emailService,
            IJobTitleService jobTitleService,
            ITeamService teamService,
            IToeicService toeicService,
            ICommandService commandService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _emailService = emailService ?? new EmailService();
            _jobTitleService = jobTitleService ?? new JobTitleService();
            _teamService = teamService ?? new TeamService();
            _toeicService = toeicService ?? new ToeicService();
            _commandService = commandService ?? new CommandService();
        }

        public ActionResult ToeicManagement(int? jobTitleId = null)
        {
            var model = new ToeicManagementViewModel
            {
                JobTitleId = jobTitleId,
                JobTitles = _jobTitleService.GetAll(),
                Teams = _teamService.GetTeam()
            };

            model.JobTitles = _jobTitleService.GetAll();
            model.Teams = _teamService.GetTeam();

            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult ToeicManagementDetail(ToeicManagementViewModel model)
        {
            IList<Employee> employees = new List<Employee>();
            if (model != null)
            {
                if (model.TeamId == -1)
                {
                    model.TeamId = null;
                }

                var getToeicScoreCriteria = new GetEmployeeToeicScoreCriteria
                {
                    Query = model.Query,
                    GroupId = model.TeamId,
                    TitleId = model.JobTitleId
                };
                employees = _employeeService.GetEmployeeToeicScore(getToeicScoreCriteria);
            }

            return PartialView("_ToeicManagementDetail", employees);
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateFullToeic(int? id, string name)
        {
            Toeic toeic = null;

            if (id.HasValue && id.Value > 0)
            {
                var criteria = new GetToeicScoreByEmployeeIdCriteria
                {
                    EmpId = id.Value
                };
                toeic = _toeicService.GetById(criteria);
                toeic = toeic ?? new Toeic { EmpId = id.Value };
            }

            ViewBag.EmployeeName = name;

            return PartialView(toeic);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateFullToeic(Toeic model)
        {
            var currentUserId = CurrentUser.Id;
            CommandResult result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };
            if (ModelState.IsValid)
            {
                ValidateTOEIC(model);

                if (model != null && ModelState.IsValid)
                {
                    var oldScore = GetToeicScore(model.EmpId);
                    var newScore = model;

                    model.CreatedBy = model.ModifiedBy = currentUserId;
                    model.CreatedDate = model.ModifiedDate = DateTime.Now;
                    var toeicCriteria = new UpdateFullToeicScoreCriteria
                    {
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now,
                        EmpId = model.EmpId,
                        InputDate = model.InputDate,
                        Listening = model.Listening,
                        LRExamDate = model.LRExamDate,
                        ModifiedBy = currentUserId,
                        ModifiedDate = DateTime.Now,
                        OldListening = model.OldListening,
                        OldReading = model.OldReading,
                        OldSpeaking = model.OldSpeaking,
                        OldWriting = model.OldWriting,
                        Reading = model.Reading,
                        Speaking = model.Speaking,
                        SWExamDate = model.SWExamDate,
                        Writing = model.Writing
                    };
                    result = _commandService.Update(toeicCriteria);

                    if (result.IsSuccessful && model.SendMail)
                    {
                        SendUpdatedScoreEmailToRelatedPeople(model.EmpId, newScore, oldScore);
                    }
                }
            }

            return Json(result);
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateToeic()
        {
            return PartialView(new ToeicViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateToeic(ToeicViewModel model)
        {
            CommandResult result = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var toeic = new Toeic
                {
                    EmpId = model.EmpId,
                    InputDate = model.InputDate,
                    LRExamDate = model.LRExamDate,
                    Listening = model.Listening,
                    Reading = model.Reading,
                    SWExamDate = model.SWExamDate,
                    Speaking = model.Speaking,
                    Writing = model.Writing,
                    CreatedBy = CurrentUser.Id,
                    ModifiedBy = CurrentUser.Id
                };

                ValidateTOEIC(toeic);

                if (ModelState.IsValid)
                {
                    var oldScore = GetToeicScore(model.EmpId);
                    var toeicScoreCriteria = new UpdateToeicScoreCriteria
                    {
                        EmpId = model.EmpId,
                        InputDate = model.InputDate,
                        LRExamDate = model.LRExamDate,
                        Listening = model.Listening,
                        Reading = model.Reading,
                        SWExamDate = model.SWExamDate,
                        Speaking = model.Speaking,
                        Writing = model.Writing,
                        CreatedBy = CurrentUser.Id,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = CurrentUser.Id,
                        ModifiedDate = DateTime.Now
                    };
                    CommandResult rerult = _commandService.Update(toeicScoreCriteria);

                    if (rerult.IsSuccessful && model.SendMail)
                    {
                        SendUpdatedScoreEmailToRelatedPeople(model.EmpId, toeic, oldScore);
                    }
                }
            }

            return Json(new { code = result.Code, error = ModelState.Values.SelectMany(v => v.Errors) }, JsonRequestBehavior.DenyGet);
        }

        private void ValidateTOEIC(Toeic model)
        {
            if (model.EmpId <= 0)
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidEmployeeID);
            }

            // check date
            DateTime currentDate = DateTime.Today;

            if (model.LRExamDate.HasValue && model.LRExamDate.Value > currentDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    string.Format(Messages.InvalidTOEICLRExamDate, currentDate.ToString("dd MMM, yyyy")));
            }

            if (model.SWExamDate.HasValue && model.SWExamDate.Value > currentDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    string.Format(Messages.InvalidTOEICSWExamDate, currentDate.ToString("dd MMM, yyyy")));
            }

            // check L/R/S/W
            if ((!model.Listening.HasValue || model.Listening == 0)
                && (!model.Reading.HasValue || model.Reading == 0)
                && (!model.Speaking.HasValue || model.Speaking == 0)
                && (!model.Writing.HasValue || model.Writing == 0))
            {
                if (model.LRExamDate.HasValue || model.SWExamDate.HasValue || !model.InputDate.HasValue)
                {
                    ModelState.AddModelError(string.Empty, Messages.EmptyLRAndSW);
                }
            }
            else
            {
                if (!ValidateNumberPair(model.Listening, model.Reading))
                {
                    ModelState.AddModelError(string.Empty, Messages.EmptyLRPair);
                }

                if (!ValidateNumberPair(model.Speaking, model.Writing))
                {
                    ModelState.AddModelError(string.Empty, Messages.EmptySWPair);
                }
            }
        }

        private bool ValidateNumberPair(int? number1, int? number2)
        {
            if ((number1.HasValue && number1.Value > 0) &&
                (number2.HasValue && number2.Value > 0))
            {
                return true;
            }

            if ((!number1.HasValue || number1.Value == 0) &&
                (!number2.HasValue || number2.Value == 0))
            {
                return true;
            }

            return false;
        }

        private void SendUpdatedScoreEmailToRelatedPeople(int empId, Toeic newScore, Toeic oldScore)
        {
            var receiverEmail = string.Empty;
            var mailCC = string.Empty;
            var nickName = string.Empty;
            var subject = Labels.UpdateTOEICScore;
            if (newScore.Listening != oldScore.Listening || newScore.Reading != oldScore.Reading)
            {
                var criteria = new GetUpdatingScoreRelatedPeopleCriteria { Id = empId };
                var employees = _employeeService.GetUpdatingScoreRelatedPeople(criteria);

                if (employees.Any())
                {
                    foreach (var employee in employees)
                    {
                        if (employee.Email.Length > 0 && employee.Id == empId)
                        {
                            receiverEmail = employee.Email;
                            nickName = employee.NickName;
                            subject = string.Format(subject, employee.NickName);
                        }
                        else if (employee.Email.Length > 0)
                        {
                            mailCC += employee.Email + ";";
                        }
                    }

                    var body = GetToeicNotificationEmailBody(nickName, newScore, oldScore);

                    _emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        receiverEmail,
                        mailCC,
                        subject,
                        body);
                }
            }
        }

        private string GetToeicNotificationEmailBody(string nickName, Toeic newScore, Toeic oldScore)
        {
            var bodyContent = new
            {
                User = nickName,
                NewListening = newScore.Listening.ToString(),
                OldListening = oldScore.Listening != null ? oldScore.Listening.ToString() : "-",
                NewReading = newScore.Reading.ToString(),
                OldReading = oldScore.Reading != null ? oldScore.Reading.ToString() : "-",
                NewTotal = newScore.LRTotal != null ? newScore.LRTotal.ToString() : "-",
                OldTotal = oldScore.LRTotal != null ? oldScore.LRTotal.ToString() : "-"
            };

            var body = Utilities.ReadTextFile(
                System.Web.HttpContext.Current.Server.MapPath(
                    "~/app_data/emails/TOEICScoreUpdatedNotification.cshtml"));

            body = string.Format(
                body,
                bodyContent.User,
                bodyContent.NewListening,
                bodyContent.OldListening,
                bodyContent.NewReading,
                bodyContent.OldReading,
                bodyContent.NewTotal,
                bodyContent.OldTotal);

            return body;
        }

        private Toeic GetToeicScore(int empid)
        {
            var criteria = new GetToeicScoreByEmployeeIdCriteria { EmpId = empid };
            var score = _toeicService.GetById(criteria) ?? new Toeic();

            return score;
        }
    }
}