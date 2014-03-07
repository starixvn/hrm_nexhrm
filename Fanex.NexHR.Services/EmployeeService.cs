// --------------------------------------------------------------------------
// <copyright file="EmployeeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Employee service.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly ITeamService _teamService;
        private IEmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._teamService = new TeamService();
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;
            this._teamService = new TeamService();
        }

        public IList<Employee> GetLeaders(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> leaders = new List<Employee>();
            if (criteria.IsValid())
            {
                leaders = this._employeeRepository.GetLeaders(criteria);
            }

            return leaders;
        }

        /// <summary>
        /// Get Employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Employee Entity.</returns>
        public Employee Get(GetEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Employee employee = new Employee();
            if (criteria.IsValid())
            {
                if ((criteria.Id != null && criteria.Id != -1) || criteria.DomainUserName != null)
                {
                    employee = this._employeeRepository.Get(criteria);
                }
            }

            return employee;
        }

        public IList<WorkingTimeRequest> GetWorkingTimeRequests(int id, int? requestType = null, byte? status = null)
        {
            throw new System.NotImplementedException();
        }

        public IList<Employee> Query(GetAllEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> lstEmployee = new List<Employee>();
            if (criteria.IsValid())
            {
                lstEmployee = this._employeeRepository.Query(criteria);
            }

            return lstEmployee;
        }

        public IList<Employee> GetEmployeeForRelatedTeam(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> lstEmployee = new List<Employee>();
            if (criteria.IsValid())
            {
                lstEmployee = this._employeeRepository.GetEmployeeForRelatedTeam(criteria);
            }

            return lstEmployee;
        }

        public int Insert(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int inserted = -1;
            if (criteria.IsValid())
            {
                inserted = this._employeeRepository.Insert(criteria);
            }

            return inserted;
        }

        public bool Delete(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool deleted = false;
            if (criteria.IsValid())
            {
                deleted = this._employeeRepository.Delete(criteria);
            }

            return deleted;
        }

        public bool Update(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._employeeRepository.Update(criteria);
            }

            return updated;
        }

        public EmployeeShift GetEmployeeShift(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            EmployeeShift model = null;
            if (criteria.IsValid())
            {
                model = this._employeeRepository.GetEmployeeShift(criteria);
            }

            return model;
        }

        public bool UpdateEmployeeShift(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._employeeRepository.UpdateEmployeeShift(criteria);
            }

            return updated;
        }

        public IList<WeekWorking> GetWeekWorking(int id, DateTime date)
        {
            Check.Argument.IsNotNull(id, "id");
            return this._employeeRepository.GetWeekWorking(id, date);
        }

        public bool UpdateWeekWorking(EmployeeShift model, DateTime date)
        {
            Check.Argument.IsNotNull(model, "model");
            return this._employeeRepository.UpdateWeekWorking(model, date);
        }

        public long WorkingTimeRequest(
            int empId, 
            short requestType, 
            bool isPlanned, 
            string remark, 
            int createdBy, 
            out int errorCode, 
            out string message, 
            DateTime fromDate, 
            int? fromShift = null, 
            DateTime? toDate = null, 
            int? toShift = null, 
            int? leaveTypeId = null, 
            decimal? totalDays = null, 
            int? lateInMins = null, 
            int? earlyOutMins = null)
        {
            throw new NotImplementedException();
        }

        public int ApproveRequest(long requestId, int approvedBy, DateTime approvedDate, int modifiedBy, out string message)
        {
            throw new NotImplementedException();
        }

        public int DisapproveRequest(long requestId, int modifiedBy, DateTime approvedDate, out string message)
        {
            throw new NotImplementedException();
        }

        public int RejectRequest(long requestId, int processedBy, DateTime processedDate, int modifiedBy, out string message)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRequest(
            long requestId, 
            bool isPlanned, 
            string remark, 
            int modifiedby, 
            DateTime fromDate, 
            int? fromShift = null, 
            DateTime? toDate = null, 
            int? toShift = null, 
            int? leaveTypeId = null, 
            decimal? totalDays = null, 
            int? lateInMins = null, 
            int? earlyOutMins = null)
        {
            throw new NotImplementedException();
        }

        public ViewModels.MySpaceViewModel GetMySpace(int id)
        {
            return this._employeeRepository.GetMySpace(id);
        }

        public HrLeaderDashboardModel GetHrLeaderDashboard(DateTime date, int? employeeId = null)
        {
            return this._employeeRepository.GetHrLeaderDashboard(date, employeeId);
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");

            return this._employeeRepository.GetWorkingSchedule(date, id);
        }

        public IList<ViewModels.WorkingCalendarsViewModel> GetWorkingCalendars(string date)
        {
            return this._employeeRepository.GetWorkingCalendars(date);
        }

        public WorkingTimeRequest GetWorkingTimeRequest(long workingTimeId)
        {
            if (workingTimeId > 0)
            {
                return this._employeeRepository.GetWorkingTimeRequest(workingTimeId);
            }

            return null;
        }

        public bool DeleteWorkingTimeRequest(int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");

            return this._employeeRepository.DeleteWorkingTimeRequest(id);
        }

        public bool UpdateShift(int id, DateTime date, int? shift, int modifiedBy, DateTime modifiedDate)
        {
            return this._employeeRepository.UpdateShift(id, date, shift, modifiedBy, modifiedDate);
        }

        public IList<CheckInOut> GetCheckInOutRequest(GetCheckInOutRequestCriteria criteria)
        {
            IList<CheckInOut> entityList = new List<CheckInOut>();

            if (criteria.IsValid())
            {
                entityList = this._employeeRepository.GetCheckInOutRequest(criteria);
            }

            return entityList;
        }

        public IList<LateInEarlyOut> GetLateInEarlyOutRequest(GetLateInOutRequestCriteria criteria)
        {
            IList<LateInEarlyOut> entityList = new List<LateInEarlyOut>();

            if (criteria.IsValid())
            {
                entityList = this._employeeRepository.GetLateInEarlyOutRequest(criteria);
            }

            return entityList;
        }

        public IList<ChangingShift> GetChangeShiftRequest(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId = null, 
            int? groupId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<ChangingShift> entityList = new List<ChangingShift>();

            if (fromDate <= toDate)
            {
                entityList = this._employeeRepository.GetChangeShiftRequest(roleId, fromDate, toDate, employeeId, groupId, status, logInId);
            }

            return entityList;
        }

        public IList<Leave> GetLeaveRequest(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId = null, 
            int? groupId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<Leave> entityList = new List<Leave>();

            if (fromDate <= toDate)
            {
                entityList = this._employeeRepository.GetLeaveRequest(roleId, fromDate, toDate, employeeId, groupId, status, logInId);
            }

            return entityList;
        }

        public IList<RequestEntity> GetAllRequest(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId = null, 
            int? groupId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            if (fromDate <= toDate)
            {
                entityList = this._employeeRepository.GetAllRequest(roleId, fromDate, toDate, employeeId, groupId, status, logInId);
            }

            return entityList;
        }

        /// <summary>
        /// Get check in/out request by date.
        /// </summary>
        /// <param name="roleId">The role ID.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="status">The status.</param>
        /// <param name="logInId">The login ID.</param>
        /// <returns>List of check in/out request.</returns>
        public IList<CheckInOut> GetCheckInOutRequestByDate(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId = null, 
            int? groupId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<CheckInOut> entityList = new List<CheckInOut>();

            if (fromDate <= toDate)
            {
                entityList = this._employeeRepository.GetCheckInOutRequestByDate(
                    roleId, 
                    fromDate, 
                    toDate, 
                    employeeId, 
                    groupId, 
                    status, 
                    logInId);
            }

            return entityList;
        }

        public IList<LateInEarlyOut> GetLateInEarlyOutRequestByDate(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId = null, 
            int? groupId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<LateInEarlyOut> entityList = new List<LateInEarlyOut>();

            if (fromDate <= toDate)
            {
                entityList = this._employeeRepository.GetLateInEarlyOutRequestByDate(roleId, fromDate, toDate, employeeId, groupId, status, logInId);
            }

            return entityList;
        }

        public IList<ChangingShift> GetChangeShiftRequestByDate(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId = null, 
            int? groupId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<ChangingShift> entityList = new List<ChangingShift>();

            if (fromDate <= toDate)
            {
                entityList = this._employeeRepository.GetChangeShiftRequestByDate(roleId, fromDate, toDate, employeeId, groupId, status, logInId);
            }

            return entityList;
        }

        public IList<Leave> GetLeaveRequestByDate(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId = null, 
            int? groupId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<Leave> entityList = new List<Leave>();

            if (fromDate <= toDate)
            {
                entityList = this._employeeRepository.GetLeaveRequestByDate(roleId, fromDate, toDate, employeeId, groupId, status, logInId);
            }

            return entityList;
        }

        public IList<RequestEntity> GetAllRequestByDate(
            short roleId, 
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId = null, 
            int? groupId = null, 
            byte? status = null, 
            int? logInId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            if (fromDate <= toDate)
            {
                entityList = this._employeeRepository.GetAllRequestByDate(roleId, fromDate, toDate, employeeId, groupId, status, logInId);
            }

            return entityList;
        }

        public IList<CheckInOut> GetCheckInOutRequestHistory(long requestId, int requestTypeId)
        {
            IList<CheckInOut> entityList = new List<CheckInOut>();

            entityList = this._employeeRepository.GetCheckInOutRequestHistory(requestId, requestTypeId);

            return entityList;
        }

        public IList<LateInEarlyOut> GetLateInEarlyOutRequestHistory(long requestId, int requestTypeId)
        {
            IList<LateInEarlyOut> entityList = new List<LateInEarlyOut>();

            entityList = this._employeeRepository.GetLateInEarlyOutRequestHistory(requestId, requestTypeId);

            return entityList;
        }

        public IList<ChangingShift> GetChangeShiftRequestHistory(long requestId, int requestTypeId)
        {
            IList<ChangingShift> entityList = new List<ChangingShift>();

            entityList = this._employeeRepository.GetChangeShiftRequestHistory(requestId, requestTypeId);

            return entityList;
        }

        public IList<Leave> GetLeaveRequestHistory(long requestId, int requestTypeId)
        {
            IList<Leave> entityList = new List<Leave>();

            entityList = this._employeeRepository.GetLeaveRequestHistory(requestId, requestTypeId);

            return entityList;
        }

        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans)
        {
            return this._employeeRepository.GetAnnualLeaveTransactions(id, year, excludedRequestTrans);
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id)
        {
            AnnualLeaveBalanceModel model = null;
            if (id > 0)
            {
                model = this._employeeRepository.GetAnnualLeaveTransaction(id);
            }

            return model;
        }

        public bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy)
        {
            return this._employeeRepository.DeleteAnnualLeaveTransaction(id, modifiedDate, modifiedBy);
        }

        public bool UpdateAnnualLeaveTransaction(
            long id,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime modifiedDate,
            int modifiedBy)
        {
            return this._employeeRepository.UpdateAnnualLeaveTransaction(id, description, fromDate, deductDays, creditDays, modifiedDate, modifiedBy);
        }

        public bool InsertAnnualLeaveTransaction(
            int empId,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy)
        {
            return this._employeeRepository.InsertAnnualLeaveTransaction(empId, description, fromDate, deductDays, creditDays, createdDate, createdBy);
        }

        public IList<UserChangedShift> GetUserChangedShift(DateTime date)
        {
            return this._employeeRepository.GetUserChangedShift(date);
        }

        public IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null)
        {
            return this._employeeRepository.GetHalfDayOffs(reportDate, employeeId, groupId);
        }

        /// <summary>
        /// Update off day for all employees.
        /// </summary>
        /// <param name="modelState">The model state.</param>
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public bool UpdateOffDayForAllEmployees(ModelStateDictionary modelState, ChangeWorkingCalendarsViewModel model)
        {
            // Check.Argument.IsNotNull(model, "model");
            bool result = true;

            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                this._employeeRepository.UpdateOffDayForAllEmployees(model.Date, model.IsOfDay, model.ModifiedBy, model.ModifiedDate, out errorCode, out message);
                if (errorCode != 0)
                {
                    modelState.AddModelError(string.Empty, message);
                    result = false;
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Get range date.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <returns>The range date.</returns>
        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return this._employeeRepository.GetRangeDate(employeeId, fromDate, toDate);
        }

        /// <summary>
        /// Get disable request date.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <returns>List of date.</returns>
        public IEnumerable<DateTime> GetDisableRequestDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return this._employeeRepository.GetDisableRequestDate(employeeId, fromDate, toDate);
        }

        /// <summary>
        /// Export total working hours.
        /// </summary>
        /// <param name="workbook">The workbook.</param>
        /// <param name="listTotalWorkingHours">List of total working hours.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public void ExportTotalWorkingHours(
            HSSFWorkbook workbook, 
            IList<TotalWorkingHours> listTotalWorkingHours, 
            DateTime fromDate, 
            DateTime toDate)
        {
            // Create a new workbook and a sheet named "User Accounts"
            // var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Total Working Hours");
            var rowTitle = sheet.CreateRow(1);
            var titleString = "Total Working Hours: " + fromDate.ToString("dd MMM, yyyy") + " - " + toDate.ToString("dd MMM, yyyy");

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var cell = rowTitle.CreateCell(1);
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 12));

            // header
            var rowIndex = 3;
            var row = sheet.CreateRow(rowIndex);
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 11;
            font1.Color = NPOI.HSSF.Util.HSSFColor.GREY_80_PERCENT.WHITE.index;
            font1.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            ICellStyle cellStyle = workbook.CreateCellStyle();

            cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_80_PERCENT.index;
            cellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.GREY_80_PERCENT.index;
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            cellStyle.VerticalAlignment = VerticalAlignment.CENTER;
            cellStyle.SetFont(font1);

            var cell1 = row.CreateCell(0);
            cell1.CellStyle = cellStyle;
            cell1.SetCellValue("No.");

            var cell2 = row.CreateCell(1);
            cell2.CellStyle = cellStyle;
            cell2.SetCellValue(Labels.Code);

            var cell3 = row.CreateCell(2);
            cell3.CellStyle = cellStyle;
            cell3.SetCellValue(Labels.Name);

            var cell4 = row.CreateCell(3);
            cell4.CellStyle = cellStyle;
            cell4.SetCellValue(Labels.Nick);

            var cell5 = row.CreateCell(4);
            cell5.CellStyle = cellStyle;
            cell5.SetCellValue(Labels.Team);

            var cell6 = row.CreateCell(5);
            cell6.CellStyle = cellStyle;
            cell6.SetCellValue(Labels.WorkigDay);

            var cell7 = row.CreateCell(6);
            cell7.CellStyle = cellStyle;
            cell7.SetCellValue(Labels.ActualWorkingDay);

            var cell8 = row.CreateCell(7);
            cell8.CellStyle = cellStyle;
            cell8.SetCellValue("Leave Taken");

            var cell9 = row.CreateCell(8);
            cell9.CellStyle = cellStyle;
            cell9.SetCellValue("Unpaid Leave");

            var cell10 = row.CreateCell(9);
            cell10.CellStyle = cellStyle;
            cell10.SetCellValue(Labels.LateIn);

            var cell11 = row.CreateCell(10);
            cell11.CellStyle = cellStyle;
            cell11.SetCellValue(Labels.EarlyOut);

            var cell12 = row.CreateCell(11);
            cell12.CellStyle = cellStyle;
            cell12.SetCellValue(Labels.TotalHours);

            var cell13 = row.CreateCell(12);
            cell13.CellStyle = cellStyle;
            cell13.SetCellValue("Average Hours");
            //// end header

            // body
            rowIndex++;

            ICellStyle bodyStyle = workbook.CreateCellStyle();

            ICellStyle bodyStyle1 = workbook.CreateCellStyle();
            bodyStyle1.Alignment = HorizontalAlignment.CENTER;
            bodyStyle1.VerticalAlignment = VerticalAlignment.CENTER;

            ICellStyle bodyStyle2 = workbook.CreateCellStyle();
            bodyStyle2.Alignment = HorizontalAlignment.CENTER;
            bodyStyle2.VerticalAlignment = VerticalAlignment.CENTER;
            bodyStyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            bodyStyle2.FillPattern = FillPatternType.SOLID_FOREGROUND;
            bodyStyle2.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;

            ICellStyle bodyStyle3 = workbook.CreateCellStyle();
            bodyStyle3.Alignment = HorizontalAlignment.LEFT;

            ICellStyle bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.LEFT;
            bodyStyle4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            bodyStyle4.FillPattern = FillPatternType.SOLID_FOREGROUND;
            bodyStyle4.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            ICell cellBody;
            //// Add data rows
            foreach (var totalWorkingHours in listTotalWorkingHours)
            {
                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                row = sheet.CreateRow(rowIndex);
                cellBody = row.CreateCell(0);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(rowIndex - 3);

                cellBody = row.CreateCell(1);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.Code.Trim());

                bodyStyle3.Alignment = HorizontalAlignment.LEFT;
                cellBody = row.CreateCell(2);
                if (rowIndex % 2 == 0)
                {
                    cellBody.CellStyle = bodyStyle4;
                }
                else
                {
                    cellBody.CellStyle = bodyStyle3;
                }

                cellBody.SetCellValue(totalWorkingHours.Employee.FullName);

                // bodyStyle.Alignment = HorizontalAlignment.CENTER;
                cellBody = row.CreateCell(3);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.NickName);

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.Team.Name);

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.WorkingDay);

                cellBody = row.CreateCell(6);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.ActualWorkingDay);

                cellBody = row.CreateCell(7);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.LeaveTaken);

                cellBody = row.CreateCell(8);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.UnpaidLeave);

                cellBody = row.CreateCell(9);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.LateInMins);

                cellBody = row.CreateCell(10);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.EarlyOutMins);

                cellBody = row.CreateCell(11);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.TotalHours.ToString("#.##"));

                cellBody = row.CreateCell(12);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.AverageHours.ToString("#.##"));

                rowIndex++;
            }
            ////end body

            sheet.CreateFreezePane(0, 4);
            //// Auto-size each column
            for (var i = 0; i < sheet.GetRow(3).LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        #region Get leave summary by team

        /// <summary>
        /// This function use for sorting leave summaries. Any leave summary of employee will be grouped by team. (For easier displaying in view).
        /// </summary>
        /// <param name="leaveSummaryList">Leave summary List.</param>
        /// <param name="teams">Team List.</param>
        /// <returns>List Annual Leave.</returns>
        public IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = new List<LeaveSummaryByTeam>();
            var criteria = new GetTeamByIdCriteria
            {
                Id = 11
            };

            if (teams.Count > 1 || (teams.Count == 1 && teams[0].Id == 11))
            {
                LeaveSummaryByTeam management = new LeaveSummaryByTeam
                {
                    Team = this._teamService.Get(criteria),
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };
                leaveSummaryByTeamList.Add(management);
            }

            foreach (Team team in teams)
            {
                LeaveSummaryByTeam entity = new LeaveSummaryByTeam
                {
                    Team = team,
                    LeaveSummaryInTeam = new List<LeaveSummary>()
                };

                foreach (LeaveSummary leavesumary in leaveSummaryList)
                {
                    if (leavesumary.Employee.Team.Name == team.Name)
                    {
                        if ((team.Id == 11 && teams.Count > 1) || (teams.Count == 1 && teams[0].Id == 11))
                        {
                            leaveSummaryByTeamList[0].LeaveSummaryInTeam.Add(leavesumary);
                        }
                        else
                        {
                            entity.LeaveSummaryInTeam.Add(leavesumary);
                        }
                    }
                }

                if (teams.Count == 1 && teams[0].Id == 11)
                {
                    return leaveSummaryByTeamList;
                }

                leaveSummaryByTeamList.Add(entity);
            }

            return leaveSummaryByTeamList;
        }
        #endregion

        public IList<Employee> GetEmployeeTOEICScore(string query, int? groupId, int? titleId = null, int? score = null, bool? enable = null, bool? isDelete = null)
        {
            return this._employeeRepository.GetEmployeeTOEICScore(query, groupId, titleId, score, enable, isDelete);
        }

        /// <summary>
        /// Function to get employees has birthday and first working.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <param name="employeesBirthday">Employees has Birthday in a date.</param>
        /// <param name="employeesFirstWorkingday">Employees has First working in a date.</param>
        public void GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria, ref IList<Employee> employeesBirthday, ref IList<Employee> employeesFirstWorkingday)
        {
            this._employeeRepository.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria, ref employeesBirthday, ref employeesFirstWorkingday);
        }
    }
}
