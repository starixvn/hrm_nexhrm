namespace Fanex.NexHR.Controllers.Request
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;

    public class LeaveController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly ILeaveTypeService leaveTypeService;
        private readonly IRequestService requestService;

        public LeaveController()
            : this(null, null, null)
        {
        }

        public LeaveController(
            IEmployeeService employeeService = null,
            ILeaveTypeService leaveTypeService = null,
            IRequestService requestService = null)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            this.requestService = requestService ?? new RequestService();
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

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Create()
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = this.CurrentUser.Id };
            var employee = employeeService.GetEmployeeById(getEmployeeCriteria);
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };
            var model = new LeaveRequestViewModel
            {
                Employee = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                By = this.CurrentUser.Id,
                LeaveTypes = leaveTypeService.GetAll().ToList(),
                UnavailDates = requestService.GetDisableRequestDate(getUnavailableRequestDateCriteria)
            };

            // Count number days of Leave request in current month.
            int year = DateTime.Now.Year,
                month = DateTime.Now.Month;
            var criteria = new GetNumberRequestInMonthCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                RequestType = (int)RequestType.Leave
            };

            ViewBag.TotalRequest = (decimal)requestService.GetNumberRequestInMonth(criteria);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(LeaveRequestViewModel model)
        {
            if (model != null)
            {
                if (model.EmployeeId <= 0)
                {
                    ModelState.AddModelError(string.Empty, Messages.InvalidEmployeeID);
                }
                else
                {
                    model.LeaveTypes = leaveTypeService.GetAll().ToList();
                    model.By = CurrentUser.Id;
                    ViewBag.Year = model.FromDate.Year;

                    var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
                    {
                        EmployeeId = model.EmployeeId,
                        FromDate = Utilities.RequestAvailableFromDate,
                        ToDate = Utilities.RequestAvailableToDate
                    };

                    model.UnavailDates = requestService.GetDisableRequestDate(getUnavailableRequestDateCriteria).ToList();
                    ViewBag.Requested = requestService.CreateLeave(ModelState, model);
                }
            }

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };
            var getRequestByIdCriteria = new GetRequestByIdCriteria() { Id = id };
            var request = requestService.GetRequestEntity(getRequestByIdCriteria);
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = request.Requester.Id };
            var employee = employeeService.GetEmployeeById(getEmployeeCriteria);
            var model = new LeaveRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                FromDate = request.FromDate.Date,
                FromTime = Utilities.GetTime(request.FromDate),
                ToDate = request.ToDate,
                ToTime = Utilities.GetTime(request.ToDate),
                TotalDays = (decimal)request.TotalDays,
                LeaveTypeId = (int)request.LeaveType.Id,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                By = this.CurrentUser.Id,
                HalfDayOff = request.TotalDays == (decimal)0.5,
                LeaveTypes = leaveTypeService.GetAll().ToList(),
                UnavailDates = requestService.GetDisableRequestDate(getUnavailableRequestDateCriteria)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(LeaveRequestViewModel model)
        {
            if (model != null)
            {
                model.LeaveTypes = leaveTypeService.GetAll().ToList();
                ViewBag.Updated = requestService.UpdateLeave(ModelState, model);
            }

            return View(model);
        }
    }
}