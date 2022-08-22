namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Criteria.Employee;
    using Criteria.Toeic;
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Resources;
    using Services.Interfaces;
    using ViewModels.Toeic;

    public class ToeicController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IEmailService emailService;
        private readonly IJobTitleService jobTitleService;
        private readonly ITeamService teamService;
        private readonly IToeicService toeicService;
        private readonly ICommandService commandService;
        private readonly IWebHostEnvironment environment;

        public ToeicController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IEmailService emailService,
            IJobTitleService jobTitleService,
            ITeamService teamService,
            IToeicService toeicService,
            ICommandService commandService,
            IWebHostEnvironment environment) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.emailService = emailService;
            this.jobTitleService = jobTitleService;
            this.teamService = teamService;
            this.toeicService = toeicService;
            this.commandService = commandService;
            this.environment = environment;
        }

        public ActionResult ToeicManagement(int? jobTitleId = null)
        {
            var model = new ToeicManagementViewModel
            {
                JobTitleId = jobTitleId,
                JobTitles = jobTitleService.GetAll(),
                Teams = teamService.GetTeam()
            };

            model.JobTitles = jobTitleService.GetAll();
            model.Teams = teamService.GetTeam();
            var userRoles = employeeService.UserRoles(CurrentUser);
            ViewBag.HasHRRole = userRoles.Any(item => item == "HR");
            return View(model);
        }

        public async Task<IActionResult> ToeicManagementDetail(ToeicManagementViewModel model)
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
                    GroupId = model.TeamId
                };

                employees = await employeeService.GetEmployeeToeicScore(getToeicScoreCriteria);
                employees = SortToeic(model.Sort, employees);
            }
            var userRoles = employeeService.UserRoles(CurrentUser);
            ViewBag.HasHRRole = userRoles.Any(item => item == "HR");
            return PartialView("_ToeicManagementDetail", employees);
        }

        private static IList<Employee> SortToeic(string sort, IList<Employee> employees)
        {
            IList<Employee> toeicSorted;

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);
                toeicSorted = Utilities.MultipleSort(employees, sorts);
            }
            else
            {
                toeicSorted = employees;
            }

            return toeicSorted;
        }

        [HttpPost]
        public ActionResult GetToeicScoreByEmpId(int empId)
        {
            var criteria = new GetToeicScoreByEmployeeIdCriteria
            {
                EmpId = empId
            };
            var toeic = toeicService.GetById(criteria) ?? new Toeic();

            return Json(toeic);
        }

        [HttpGet]
        [HrmAuthorize(Roles = "HR")]
        public ActionResult UpdateToeic(int? id, string name)
        {
            Toeic toeic = null;

            if (id.HasValue && id.Value > 0)
            {
                var criteria = new GetToeicScoreByEmployeeIdCriteria
                {
                    EmpId = id.Value
                };
                toeic = toeicService.GetById(criteria);
                toeic ??= new Toeic { EmpId = id.Value };
                toeic.Employee ??= name;
            }

            return PartialView(toeic);
        }

        [HttpPost]
        [HrmAuthorize(Roles = "HR")]
        public ActionResult UpdateToeic(Toeic model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };
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
                    MinimumScore = model.MinimumScore,
                    CreatedBy = CurrentUser.Id,
                    ModifiedBy = CurrentUser.Id
                };

                ValidateTOEIC(toeic);

                if (ModelState.IsValid)
                {
                    var oldScore = GetToeicScore(model.EmpId);
                    var toeicScoreCriteria = new UpdateToeicScoreCriteria
                    {
                        CreatedBy = CurrentUser.Id,
                        CreatedDate = DateTime.Now,
                        EmpId = model.EmpId,
                        InputDate = model.InputDate,
                        Listening = model.Listening,
                        LRExamDate = model.LRExamDate,
                        ModifiedBy = CurrentUser.Id,
                        ModifiedDate = DateTime.Now,
                        OldListening = model.OldListening,
                        OldReading = model.OldReading,
                        OldSpeaking = model.OldSpeaking,
                        OldWriting = model.OldWriting,
                        Reading = model.Reading,
                        Speaking = model.Speaking,
                        SWExamDate = model.SWExamDate,
                        Writing = model.Writing,
                        MinimumScore = model.MinimumScore
                    };
                    result = commandService.ExecuteNonQuery(toeicScoreCriteria);

                    if (result.IsSuccessful && model.SendMail)
                    {
                        SendUpdatedScoreEmailToRelatedPeople(model.EmpId, toeic, oldScore);
                    }

                    if (!result.IsSuccessful)
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return Json(new { code = result.Code, error = ModelState.Values.SelectMany(v => v.Errors) });
        }

        private void ValidateTOEIC(Toeic model)
        {
            if (model.EmpId <= 0)
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidEmployeeID);
            }

            CheckTOEICDate(model);

            CheckTOEICScores(model);
        }

        private static bool IsSubToeicScoreValid(int? score)
        {
            return score != null && score.Value != 0;
        }

        private void CheckTOEICScores(Toeic model)
        {
            // check L/R/S/W
            if (!IsSubToeicScoreValid(model.Listening)
                && !IsSubToeicScoreValid(model.Reading)
                && !IsSubToeicScoreValid(model.Speaking)
                && !IsSubToeicScoreValid(model.Writing))
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

        private void CheckTOEICDate(Toeic model)
        {
            if (model.LRExamDate.HasValue && model.LRExamDate.Value > DateTime.Today)
            {
                ModelState.AddModelError(
                    string.Empty,
                    string.Format(Messages.InvalidTOEICLRExamDate, DateTime.Today.ToString("dd MMM, yyyy")));
            }

            if (model.SWExamDate.HasValue && model.SWExamDate.Value > DateTime.Today)
            {
                ModelState.AddModelError(
                    string.Empty,
                    string.Format(Messages.InvalidTOEICSWExamDate, DateTime.Today.ToString("dd MMM, yyyy")));
            }
        }

        private static bool ValidateNumberPair(int? number1, int? number2)
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
            var mailCC = new StringBuilder();
            var nickName = string.Empty;
            var subject = Labels.UpdateTOEICScore;

            if (newScore.Listening == oldScore.Listening && newScore.Reading == oldScore.Reading)
            {
                return;
            }

            var criteria = new GetUpdatingScoreRelatedPeopleCriteria { Id = empId };
            var employees = employeeService.GetUpdatingScoreRelatedPeople(criteria);

            if (!employees.Any())
            {
                return;
            }

            foreach (var employee in employees)
            {
                if (employee.Email.Length <= 0)
                {
                    continue;
                }

                if (employee.Id == empId)
                {
                    receiverEmail = employee.Email;
                    nickName = employee.NickName;
                    subject = string.Format(subject, employee.NickName);
                }

                mailCC.Append(employee.Email);
                mailCC.Append(";");
            }

            var body = GetToeicNotificationEmailBody(nickName, newScore, oldScore);

            emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                receiverEmail,
                mailCC.ToString(),
                subject,
                body);
        }

        private static string GetDisplayScore(int? score)
        {
            return score != null ? score.ToString() : "-";
        }

        private string GetToeicNotificationEmailBody(string nickName, Toeic newScore, Toeic oldScore)
        {
            var bodyContent = new
            {
                User = nickName,
                NewListening = newScore.Listening.ToString(),
                OldListening = GetDisplayScore(oldScore.Listening),
                NewReading = newScore.Reading.ToString(),
                OldReading = GetDisplayScore(oldScore.Reading),
                NewTotal = GetDisplayScore(newScore.LRTotal),
                OldTotal = GetDisplayScore(oldScore.LRTotal)
            };

            var templatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForTOEICScoreUpdatedNotification);

            var body = Utilities.ReadTextFile(templatePath);

            body = string.Format(
                body,
                bodyContent.User,
                bodyContent.NewListening,
                bodyContent.OldListening,
                bodyContent.NewReading,
                bodyContent.OldReading,
                bodyContent.NewTotal,
                bodyContent.OldTotal,
                ConfigurationManager.AppSettings["CompanyMailSign"]);

            return body;
        }

        private Toeic GetToeicScore(int empid)
        {
            var criteria = new GetToeicScoreByEmployeeIdCriteria { EmpId = empid };
            var score = toeicService.GetById(criteria) ?? new Toeic();

            return score;
        }
    }
}