// --------------------------------------------------------------------------
// <copyright file="ToeicController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.01.05">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Criteria.Toeic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class ToeicController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmailService _emailService;
        private readonly ILookupDataService _lookupDataService;
        private readonly ITeamService _teamService;
        private readonly IToeicService _toeicService;

        public ToeicController()
            : this(null, null, null, null, null)
        {
        }

        public ToeicController(
            IEmployeeService employeeService = null,
            IEmailService emailService = null,
            ILookupDataService lookupDataService = null,
            ITeamService teamService = null,
            IToeicService toeicService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _emailService = emailService ?? new EmailService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _teamService = teamService ?? new TeamService();
            _toeicService = toeicService ?? new ToeicService();
        }

        public ActionResult ToeicManagement(int? jobTitleId = null)
        {
            var model = new ToeicManagementViewModel()
            {
                JobTitleId = jobTitleId
            };

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria()
            {
                LookupCode = LookupCodeName.JobTitle
            };

            model.JobTitles = _lookupDataService.GetAll(getAllLookupDataCriteria);
            model.Groups = _teamService.GetAll(new GetAllTeamCriteria());

            return View(model);
        }

        public ActionResult ToeicManagementDetail(ToeicManagementViewModel model)
        {
            if (model.GroupId == -1)
            {
                model.GroupId = null;
            }

            var getToeicScoreCriteria = new GetEmployeeToeicScoreCriteria
            {
                Query = model.Query,
                GroupId = model.GroupId,
                TitleId = model.JobTitleId
            };

            IList<Employee> employees = _employeeService.GetEmployeeToeicScore(getToeicScoreCriteria);

            return PartialView("_ToeicManagementDetail", employees);
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateFullToeic(int? id, string name)
        {
            Toeic toeic = null;

            if (id.HasValue && id.Value > 0)
            {
                var criteria = new GetToeicScoreByEmployeeIdCriteria()
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
            int effectedRow = -1;
            var currentUserId = this.CurrentUser.Id;
            if (ModelState.IsValid)
            {
                ValidateTOEIC(model);

                if (ModelState.IsValid)
                {
                    var oldScore = GetToeicScore(model.EmpId);
                    var newScore = model;

                    model.CreatedBy = model.ModifiedBy = currentUserId;
                    model.CreatedDate = model.ModifiedDate = DateTime.Now;
                    var toeicCriteria = new UpdateFullToeicScoreCriteria()
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
                    effectedRow = _toeicService.UpdateFullToeicScore(toeicCriteria);

                    if (effectedRow > 0)
                    {
                        SendUpdatedScoreEmailToRelatedPeople(model.EmpId, newScore, oldScore);
                    }
                }
            }

            return Json(new { code = effectedRow, error = ModelState.Values.SelectMany(v => v.Errors) }, JsonRequestBehavior.DenyGet);
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
            int effectedRow = -1;
            if (ModelState.IsValid)
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
                    var toeicScoreCriteria = new UpdateToeicScoreCriteria()
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
                    effectedRow = _toeicService.UpdateToeicScore(toeicScoreCriteria);

                    if (effectedRow > 0)
                    {
                        SendUpdatedScoreEmailToRelatedPeople(model.EmpId, toeic, oldScore);
                    }
                }
            }

            return Json(new { code = effectedRow, error = ModelState.Values.SelectMany(v => v.Errors) }, JsonRequestBehavior.DenyGet);
        }

        private void ValidateTOEIC(Toeic model)
        {
            if (model.EmpId <= 0)
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidEmployeeID);
            }

            // check date
            DateTime currentDate = DateTime.Today;

            if (model.LRExamDate.HasValue && model.LRExamDate.Value > currentDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    string.Format(Resources.Messages.InvalidTOEICLRExamDate, currentDate.ToString("dd MMM, yyyy")));
            }

            if (model.SWExamDate.HasValue && model.SWExamDate.Value > currentDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    string.Format(Resources.Messages.InvalidTOEICSWExamDate, currentDate.ToString("dd MMM, yyyy")));
            }

            // check L/R/S/W
            if ((!model.Listening.HasValue || model.Listening == 0) &&
                (!model.Reading.HasValue || model.Reading == 0) &&
                (!model.Speaking.HasValue || model.Speaking == 0) &&
                (!model.Writing.HasValue || model.Writing == 0))
            {
                if (model.LRExamDate.HasValue || model.SWExamDate.HasValue || !model.InputDate.HasValue)
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.EmptyLRAndSW);
                }
            }
            else
            {
                if (!ValidateNumberPair(model.Listening, model.Reading))
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.EmptyLRPair);
                }

                if (!ValidateNumberPair(model.Speaking, model.Writing))
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.EmptySWPair);
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
            else if ((!number1.HasValue || number1.Value == 0) &&
                (!number2.HasValue || number2.Value == 0))
            {
                return true;
            }

            return false;
        }

        private UpdateEmployeeCriteria FillDataUpdateEmployeeCriteria(Employee model)
        {
            var criteria = new UpdateEmployeeCriteria()
            {
                Code = model.Code,
                ConfirmedDate = model.ConfirmedDate,
                CurrAddress = model.CurrentAddress,
                Address = model.Address,
                Distancetocompany = model.DistanceToCompany,
                DateOfBirth = model.DateOfBirth,
                DomainUsername = model.DomainUsername,
                Email = model.Email,
                Enabled = model.Enabled,
                FullName = model.FullName,
                Gender = model.Gender,
                Geomaplatitude = model.GeoMapLatitude,
                Geomaplongtitude = model.GeoMapLongtitude,
                HandleRequestGroupIds = model.LeaderGroups,
                Id = model.Id,
                IdCardNo = model.CardNo,
                IssuedDate = model.IssuedDate,
                IssuedPlace = model.IssuedPlace,
                JobTitleId = model.JobTitleId,
                JoinedDate = model.JoinedDate,
                MainGroupId = model.Team.Id,
                Mariage = model.Mariage,
                ModifiedBy = model.Modifier.Id,
                ModifiedDate = DateTime.Now.Date,
                NickName = model.NickName,
                PassportExp = model.PassportExp,
                PassportNo = model.PassportNo,
                Phone = model.Phone,
                ResignedDate = model.ResignedDate,
                Roles = model.Roles,
                Skype = model.Skype,
                SubGroups = model.SubGroups,
                Yahoo = model.Yahoo,
                EmployeeCard = model.EmployeeCard
            };

            return criteria;
        }

        private void SendUpdatedScoreEmailToRelatedPeople(int empId, Toeic newScore, Toeic oldScore)
        {
            var receiverEmail = string.Empty;
            var mailCC = string.Empty;
            var nickName = string.Empty;
            var subject = Labels.UpdateTOEICScore;
            if (newScore.Listening != oldScore.Listening || newScore.Reading != oldScore.Reading)
            {
                var criteria = new GetUpdatingScoreRelatedPeopleCriteria() { Id = empId };
                var employees = _employeeService.GetUpdatingScoreRelatedPeople(criteria);

                if (employees.Count() > 0)
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

                    var body = Utilities.ReadTextFile(
                        System.Web.HttpContext.Current.Server.MapPath("~/app_data/emails/TOEICScoreUpdatedNotification.cshtml"));
                    var bodyContent = new
                    {
                        User = nickName,
                        NewListening = newScore.Listening.ToString(),
                        OldListening = oldScore.Listening != null ? oldScore.Listening.ToString() : "-",
                        NewReading = newScore.Reading.ToString(),
                        OldReading = oldScore.Reading != null ? oldScore.Reading.ToString() : "-"
                    };

                    body = string.Format(body, bodyContent.User, bodyContent.NewListening, bodyContent.OldListening, bodyContent.NewReading, bodyContent.OldReading);

                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        receiverEmail,
                        mailCC,
                        subject,
                        body);
                }
            }
        }

        private Toeic GetToeicScore(int empid)
        {
            var criteria = new GetToeicScoreByEmployeeIdCriteria() { EmpId = empid };
            var score = _toeicService.GetById(criteria);
            if (score == null)
            {
                score = new Toeic();
            }

            return score;
        }
    }
}