namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class LeaveController : BaseController
    {
        private readonly ILeaveTypeService leaveTypeService;
        private readonly IRequestService requestService;
        private readonly ICommandService commandService;

        public LeaveController()
            : this(null, null, null)
        {
        }

        public LeaveController(
            ILeaveTypeService leaveTypeService,
            IRequestService requestService,
            ICommandService commandService)
        {
            this.leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            this.requestService = requestService ?? new RequestService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult EmployeeView()
        {
            return View();
        }

        public ActionResult ListLeaveRequest(int year)
        {
            var criteria = new GetRequestCriteria()
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.Leave),
                IsFilterByEffectedDate = true
            };

            IList<RequestEntity> leaveRequests = requestService.GetRequest(criteria);

            return PartialView(leaveRequests);
        }

        public ActionResult Create()
        {
            var model = new LeaveRequestViewModel
            {
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                LeaveTypes = leaveTypeService.GetAll(),
                UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id),
                TotalRequest = requestService.GetNumberRequestInMonth(CurrentUser.Id, RequestType.Leave)
            };

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(LeaveRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model = PrepareModel(model);
                if (model.EmployeeId != 0)
                {
                    var criteria = new InsertRequestCriteria()
                    {
                        EmpId = model.EmployeeId,
                        RequestType = (short)RequestType.Leave,
                        IsPlanned = model.IsPlanned,
                        Remark = model.Remark,
                        CreatedDate = DateTime.Now,
                        CreatedBy = CurrentUser.Id,
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        LeaveTypeId = model.LeaveTypeId,
                        TotalDays = model.TotalDays
                    };

                    CommandResult result = commandService.Insert(criteria, hasGetOutputParams: true);
                    model.Id = criteria.Id;
                    if (result.IsSuccessful)
                    {
                        ViewBag.Requested = true;
                        model.IsEdit = false;
                        model.GetTemplatePath();

                        Task.Factory.StartNew(() =>
                        {
                            requestService.SendMailForLeave(model);
                        });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Employee !");
                }
            }

            return View(model);
        }

        private LeaveRequestViewModel PrepareModel(LeaveRequestViewModel model)
        {
            model.LeaveTypes = leaveTypeService.GetAll();
            model.UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id);
            ViewBag.Year = model.FromDate.Year;
            if (model.HalfDayOff)
            {
                model.TotalDays = (decimal)0.5;
                model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
            }
            
            return model;
        }

        public ActionResult Update(int id)
        {
            var criteria = new GetRequestByIdCriteria() { Id = id };
            RequestEntity request = requestService.GetRequestEntity(criteria);
            var model = new LeaveRequestViewModel
            {
                Id = request.Id,
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                FromDate = request.FromDate.Date,
                FromTime = Utilities.GetTime(request.FromDate),
                ToDate = request.ToDate,
                ToTime = Utilities.GetTime(request.ToDate),
                TotalDays = (decimal)request.TotalDays,
                LeaveTypeId = (int)request.LeaveType.Id,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                HalfDayOff = request.TotalDays == (decimal)0.5,
                LeaveTypes = leaveTypeService.GetAll(),
                UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id)
            };

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Update(LeaveRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.LeaveTypes = leaveTypeService.GetAll();
                if (model.HalfDayOff)
                {
                    model.TotalDays = (decimal)0.5;
                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }

                var criteria = new UpdateRequestCriteria()
                {
                    Id = model.Id,
                    IsPlanned = model.IsPlanned,
                    Remark = model.Remark,
                    ModifiedBy = model.EmployeeId,
                    FromDate = model.FromDate,
                    ModifiedDate = DateTime.Now,
                    ToDate = model.ToDate,
                    LeaveTypeId = model.LeaveTypeId,
                    TotalDays = model.TotalDays
                };

                CommandResult result = commandService.Update(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                    model.IsEdit = true;
                    model.GetTemplatePath();

                    Task.Factory.StartNew(() =>
                    {
                        requestService.SendMailForLeave(model);
                    });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }
    }
}