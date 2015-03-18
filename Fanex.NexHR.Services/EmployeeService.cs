// --------------------------------------------------------------------------
// <copyright file="EmployeeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.Data;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Employee service.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            this._employeeRepository = new EmployeeRepository();
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;
        }

        public IList<Employee> GetLeaders(GetLeadersOfEmployeeCriteria criteria)
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
        public Employee GetEmployeeById(GetEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Employee employee = new Employee();
            if (criteria.IsValid())
            {
                if ((criteria.Id != null && criteria.Id != -1) || criteria.DomainUserName != null)
                {
                    employee = this._employeeRepository.GetById(criteria);
                }
            }

            return employee;
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

        public IList<Employee> GetEmployeeForRelatedTeam(GetEmployeeForRelatedTeamCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> lstEmployee = new List<Employee>();
            if (criteria.IsValid())
            {
                lstEmployee = this._employeeRepository.GetEmployeeForRelatedTeam(criteria);
            }

            return lstEmployee;
        }

        public int Insert(InsertEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int inserted = -1;
            if (criteria.IsValid())
            {
                inserted = this._employeeRepository.Insert(criteria);
            }

            return inserted;
        }

        public bool Delete(DeleteEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool deleted = false;
            if (criteria.IsValid())
            {
                deleted = this._employeeRepository.Delete(criteria);
            }

            return deleted;
        }

        public bool Update(UpdateEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._employeeRepository.Update(criteria);
            }

            return updated;
        }

        public EmployeeShift GetEmployeeShift(GetEmployeeShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            EmployeeShift model = null;
            if (criteria.IsValid())
            {
                model = this._employeeRepository.GetEmployeeShift(criteria);
            }

            return model;
        }

        public bool UpdateEmployeeShift(UpdateEmployeeShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._employeeRepository.UpdateEmployeeShift(criteria);
            }

            return updated;
        }

        public MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            MySpaceViewModel space = new MySpaceViewModel();
            if (criteria.IsValid())
            {
                space = this._employeeRepository.GetMySpace(criteria);
            }

            return space;
        }

        public HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            HRLeaderDashboardModel dashboard = new HRLeaderDashboardModel();
            if (criteria.IsValid())
            {
                dashboard = this._employeeRepository.GetHRLeaderDashboard(criteria);
            }

            return dashboard;
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingScheduleModel> workingSchedule = new List<WorkingScheduleModel>();
            if (criteria.IsValid())
            {
                workingSchedule = this._employeeRepository.GetWorkingSchedule(criteria);
            }

            return workingSchedule;
        }

        public IList<ViewModels.WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<WorkingCalendarsViewModel> workingCalendar = new List<WorkingCalendarsViewModel>();
            if (criteria.IsValid())
            {
                workingCalendar = this._employeeRepository.GetWorkingCalendars(criteria);
            }

            return workingCalendar;
        }

        public WorkingTimeRequest GetWorkingTimeRequest(GetWorkingTimeRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            WorkingTimeRequest workingTimeRequest = new WorkingTimeRequest();
            if (criteria.IsValid())
            {
                workingTimeRequest = this._employeeRepository.GetWorkingTimeRequest(criteria);
            }

            return workingTimeRequest;
        }

        public bool DeleteWorkingTimeRequest(DeleteWorkingTimeRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool deleted = false;
            if (criteria.IsValid())
            {
                deleted = this._employeeRepository.DeleteWorkingTimeRequest(criteria);
            }

            return deleted;
        }

        public bool UpdateShift(UpdateShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._employeeRepository.UpdateShift(criteria);
            }

            return updated;
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

        public IList<Leave> GetLeaveRequest(GetLeaveRequestCriteria criteria)
        {
            IList<Leave> entityList = new List<Leave>();

            if (criteria.IsValid())
            {
                entityList = this._employeeRepository.GetLeaveRequest(criteria);
            }

            return entityList;
        }

        public IList<RequestEntity> GetAllRequest(GetAllRequestCriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            if (criteria.IsValid())
            {
                entityList = this._employeeRepository.GetAllRequest(criteria);
            }

            return entityList;
        }

        /// <summary>
        /// Get check in/out request by date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of check in/out request.</returns>
        public IList<CheckInOut> GetCheckInOutRequestByDate(GetCheckInOutRequestByDateCriteria criteria)
        {
            IList<CheckInOut> entityList = new List<CheckInOut>();

            if (criteria.IsValid())
            {
                entityList = this._employeeRepository.GetCheckInOutRequestByDate(criteria);
            }

            return entityList;
        }

        public IList<LateInEarlyOut> GetLateInEarlyOutRequestByDate(GetLateInOutRequestByDateCriteria criteria)
        {
            IList<LateInEarlyOut> entityList = new List<LateInEarlyOut>();

            if (criteria.IsValid())
            {
                entityList = this._employeeRepository.GetLateInEarlyOutRequestByDate(criteria);
            }

            return entityList;
        }

        public IList<Leave> GetLeaveRequestByDate(GetLeaveRequestByDateCriteria criteria)
        {
            IList<Leave> entityList = new List<Leave>();

            if (criteria.IsValid())
            {
                entityList = this._employeeRepository.GetLeaveRequestByDate(criteria);
            }

            return entityList;
        }

        public IList<RequestEntity> GetAllRequestByDate(GetAllRequestByDateCriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            if (criteria.IsValid())
            {
                entityList = this._employeeRepository.GetAllRequestByDate(criteria);
            }

            return entityList;
        }

        public bool InsertWorkingCalendar(InsertEmployeeCalendarCriteria criteria)
        {
            if (criteria.IsValid())
            {
                return this._employeeRepository.InsertWorkingCalendar(criteria);
            }

            return false;
        }

        public IList<CheckInOut> GetCheckInOutRequestHistory(GetRequestHistoryCriteria criteria)
        {
            IList<CheckInOut> entityList = new List<CheckInOut>();

            entityList = this._employeeRepository.GetCheckInOutRequestHistory(criteria);

            return entityList;
        }

        public IList<LateInEarlyOut> GetLateInEarlyOutRequestHistory(GetRequestHistoryCriteria criteria)
        {
            IList<LateInEarlyOut> entityList = new List<LateInEarlyOut>();

            entityList = this._employeeRepository.GetLateInEarlyOutRequestHistory(criteria);

            return entityList;
        }

        public IList<Leave> GetLeaveRequestHistory(GetRequestHistoryCriteria criteria)
        {
            IList<Leave> entityList = new List<Leave>();

            entityList = this._employeeRepository.GetLeaveRequestHistory(criteria);

            return entityList;
        }

        public ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<ShowOpenAndCloseAnnualBalance>(criteria.GetSpParams());

                return entity;
            }
        }

        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            return this._employeeRepository.GetAnnualLeaveTransactions(criteria);
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            AnnualLeaveBalanceModel model = null;

            if (criteria.IsValid())
            {
                model = this._employeeRepository.GetAnnualLeaveTransactionById(criteria);
            }

            return model;
        }

        public bool DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria)
        {
            return this._employeeRepository.DeleteAnnualLeaveTransaction(criteria);
        }

        public bool UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria)
        {
            return this._employeeRepository.UpdateAnnualLeaveTransaction(criteria);
        }

        public bool InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria)
        {
            return this._employeeRepository.InsertAnnualLeaveTransaction(criteria);
        }

        public IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria)
        {
            return this._employeeRepository.GetUserChangedShift(criteria);
        }

        public IList<HalfDayOff> GetHalfDayOffs(GetHalfDayOffCriteria criteria)
        {
            return this._employeeRepository.GetHalfDayOffs(criteria);
        }

        /// <summary>
        /// Update off day for all employees.
        /// </summary>
        /// <param name="modelState">The model state.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The result.</returns>
        public bool UpdateOffDayForAllEmployees(
            ModelStateDictionary modelState, 
            UpdateOffDayForAllEmployeesCriteria criteria)
        { 
            bool result = true;

            if (modelState.IsValid)
            {
                Hashtable outputParams = this._employeeRepository.UpdateOffDayForAllEmployees(criteria);

                if ((int)outputParams["errcode"] != 0)
                {
                    modelState.AddModelError(string.Empty, (string)outputParams["errmsg"]);
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
        /// <param name="criteria">The criteria.</param>
        /// <returns>The range date.</returns>
        public int GetRangeDate(GetDateRangeEmployeeCalendarCriteria criteria)
        {
            return this._employeeRepository.GetRangeDate(criteria);
        }

        /// <summary>
        /// Get disable request date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of date.</returns>
        public IEnumerable<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria)
        {
            return this._employeeRepository.GetDisableRequestDate(criteria);
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
        /// <param name="listTeam">Team List.</param>
        /// <returns>List Annual Leave.</returns>
        public IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> listTeam)
        {
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = new List<LeaveSummaryByTeam>();

            listTeam = listTeam.OrderBy(t => t.Rank).ToList();

            foreach (Team team in listTeam)
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
                        entity.LeaveSummaryInTeam.Add(leavesumary);
                    }
                }

                leaveSummaryByTeamList.Add(entity);
            }

            return leaveSummaryByTeamList;
        }
        #endregion

        public IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria)
        {
            if (criteria.Enable != null)
            {
                criteria.Enable = criteria.Enable == true ? false : true;
            }

            return this._employeeRepository.GetEmployeeToeicScore(criteria);
        }

        /// <summary>
        /// Function to get employees has birthday and first working.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Of employee has birthday and list employee has first working day.</returns>
        public SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria)
        {
            SystemReminderModel lstEmployee = null;
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                lstEmployee = this._employeeRepository.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
            }
            
            return lstEmployee;
        }

        /// <summary>
        /// Function to get all related people in updating TOEIC score.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of related people.</returns>
        public IList<Employee> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> lstEmployee = new List<Employee>();
            if (criteria.IsValid())
            {
                lstEmployee = this._employeeRepository.GetUpdatingScoreRelatedPeople(criteria);
            }

            return lstEmployee;
        }

        /// <summary>
        /// Export employee.
        /// </summary>
        /// <param name="workbook">The workbook.</param>
        /// <param name="listEmployees">List of employees.</param>
        public void ExportEmployeeProfile(
            HSSFWorkbook workbook,
            IList<Employee> listEmployees)
        {
            ISheet sheet = workbook.CreateSheet("Employee Profile");
            var rowTitle = sheet.CreateRow(1);
            var titleString = "Employee Profile";

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var cell = rowTitle.CreateCell(1);
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 23));

            // header
            var rowIndex = 3;
            IRow row = sheet.CreateRow(rowIndex);
            RenderHeaderExportEmployeeProfile(workbook, row);
            //// end header

            // body
            RenderBodyExportEmployeeProfile(listEmployees, workbook, sheet, row);
            ////end body

            sheet.CreateFreezePane(0, 4);
            //// Auto-size each column
            for (var i = 0; i < sheet.GetRow(3).LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// Render header.
        /// </summary>
        /// <param name="workbook">The workbook.</param>
        /// <param name="row">The row of file.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderHeaderExportEmployeeProfile(HSSFWorkbook workbook, IRow row)
        {
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
            cell1.SetCellValue("#");

            var cell2 = row.CreateCell(1);
            cell2.CellStyle = cellStyle;
            cell2.SetCellValue("Emp. ID");

            var cell3 = row.CreateCell(2);
            cell3.CellStyle = cellStyle;
            cell3.SetCellValue(Labels.FullName);

            var cell4 = row.CreateCell(3);
            cell4.CellStyle = cellStyle;
            cell4.SetCellValue(Labels.Nick);

            var cell5 = row.CreateCell(4);
            cell5.CellStyle = cellStyle;
            cell5.SetCellValue("Date of Birth");

            var cell6 = row.CreateCell(5);
            cell6.CellStyle = cellStyle;
            cell6.SetCellValue(Labels.Email);

            var cell7 = row.CreateCell(6);
            cell7.CellStyle = cellStyle;
            cell7.SetCellValue(Labels.JobTitle);

            var cell8 = row.CreateCell(7);
            cell8.CellStyle = cellStyle;
            cell8.SetCellValue(Labels.Team);

            var cell9 = row.CreateCell(8);
            cell9.CellStyle = cellStyle;
            cell9.SetCellValue(Labels.JoinedDate);

            var cell10 = row.CreateCell(9);
            cell10.CellStyle = cellStyle;
            cell10.SetCellValue("Confirmed Date");

            var cell11 = row.CreateCell(10);
            cell11.CellStyle = cellStyle;
            cell11.SetCellValue("Resigned Date");

            var cell12 = row.CreateCell(11);
            cell12.CellStyle = cellStyle;
            cell12.SetCellValue(Labels.Phone);

            var cell13 = row.CreateCell(12);
            cell13.CellStyle = cellStyle;
            cell13.SetCellValue(Labels.Gender);

            var cell14 = row.CreateCell(13);
            cell14.CellStyle = cellStyle;
            cell14.SetCellValue("Card No");

            var cell15 = row.CreateCell(14);
            cell15.CellStyle = cellStyle;
            cell15.SetCellValue("Issued Date");

            var cell16 = row.CreateCell(15);
            cell16.CellStyle = cellStyle;
            cell16.SetCellValue("Issued Place");

            var cell17 = row.CreateCell(16);
            cell17.CellStyle = cellStyle;
            cell17.SetCellValue("Current Add");

            var cell18 = row.CreateCell(17);
            cell18.CellStyle = cellStyle;
            cell18.SetCellValue("Permanent Add");

            var cell19 = row.CreateCell(18);
            cell19.CellStyle = cellStyle;
            cell19.SetCellValue("Passport No.");

            var cell20 = row.CreateCell(19);
            cell20.CellStyle = cellStyle;
            cell20.SetCellValue("Passport Exp.");

            var cell21 = row.CreateCell(20);
            cell21.CellStyle = cellStyle;
            cell21.SetCellValue("Listening");

            var cell22 = row.CreateCell(21);
            cell22.CellStyle = cellStyle;
            cell22.SetCellValue("Reading");

            var cell23 = row.CreateCell(22);
            cell23.CellStyle = cellStyle;
            cell23.SetCellValue("Speaking");

            var cell24 = row.CreateCell(23);
            cell24.CellStyle = cellStyle;
            cell24.SetCellValue("Writing");
        }

        /// <summary>
        /// Render body.
        /// </summary>
        /// <param name="listEmployees">List of employees.</param>
        /// <param name="workbook">The workbook.</param>
        /// <param name="sheet">The sheet.</param>
        /// <param name="row">The row of file.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderBodyExportEmployeeProfile(IList<Employee> listEmployees, HSSFWorkbook workbook, ISheet sheet, IRow row)
        {
            var rowIndex = 4;
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
            foreach (var employee in listEmployees)
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
                cellBody.SetCellValue(FormatString(employee.Code));

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle4;
                }
                else
                {
                    bodyStyle = bodyStyle3;
                }

                cellBody = row.CreateCell(2);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.FullName));

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                cellBody = row.CreateCell(3);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.NickName));

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employee.DateOfBirth.ToString("dd-mm-yyyy"));

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle4;
                }
                else
                {
                    bodyStyle = bodyStyle3;
                }

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.Email));

                cellBody = row.CreateCell(6);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.JobTitle));

                cellBody = row.CreateCell(7);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.Team.Name));

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                cellBody = row.CreateCell(8);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatDate(employee.JoinedDate));

                cellBody = row.CreateCell(9);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatDate(employee.ConfirmedDate));

                cellBody = row.CreateCell(10);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatDate(employee.ResignedDate));

                cellBody = row.CreateCell(11);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.Phone));

                cellBody = row.CreateCell(12);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employee.Gender == 0 ? "F" : "M");

                cellBody = row.CreateCell(13);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.CardNo));

                cellBody = row.CreateCell(14);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatDate(employee.IssuedDate));

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle4;
                }
                else
                {
                    bodyStyle = bodyStyle3;
                }

                cellBody = row.CreateCell(15);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.IssuedPlace));

                cellBody = row.CreateCell(16);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.CurrentAddress));

                cellBody = row.CreateCell(17);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.PermanentAddress));

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                cellBody = row.CreateCell(18);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatString(employee.PassportNo));

                cellBody = row.CreateCell(19);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(FormatDate(employee.PassportExp));

                cellBody = row.CreateCell(20);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employee.ToeicScore == null ? string.Empty : FormatToeicScore(employee.ToeicScore.Listening));

                cellBody = row.CreateCell(21);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employee.ToeicScore == null ? string.Empty : FormatToeicScore(employee.ToeicScore.Reading));

                cellBody = row.CreateCell(22);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employee.ToeicScore == null ? string.Empty : FormatToeicScore(employee.ToeicScore.Speaking));

                cellBody = row.CreateCell(23);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employee.ToeicScore == null ? string.Empty : FormatToeicScore(employee.ToeicScore.Writing));

                rowIndex++;
            }
        }

        private dynamic FormatDate(DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }

            return date.Value.ToString("dd-mm-yyyy");
        }

        private dynamic FormatToeicScore(int? score)
        {
            if (score == null)
            {
                return string.Empty;
            }

            return score.Value;
        }

        private string FormatString(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Trim();
        }
    }
}
