// -----------------------------------------------------------------------
// <copyright file="ExportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015/7/2">Created.</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Export Service.
    /// </summary>
    public class ExportService
    {
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

        /// <summary>
        /// Export employee attendance.
        /// </summary>
        /// <param name="workbook">The workbook.</param>
        /// <param name="listEmployeeAttendances">List of employee attendance.</param>
        public void ExportEmployeeAttendances(HSSFWorkbook workbook, IList<EmployeeAttendanceModel> listEmployeeAttendances)
        {
            ISheet sheet = workbook.CreateSheet("Employee Attendance");
            var rowTitle = sheet.CreateRow(1);
            var titleString = "Employee Attendance";

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var cell = rowTitle.CreateCell(1);
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 12));

            // header
            var titleHeaders = new List<string>
            {
                "No.",
                Labels.Date,
                Labels.Code,
                Labels.Nick,
                Labels.FullName,
                Labels.Team,
                Labels.Shift,
                Labels.CheckIn,
                Labels.CheckOut,
                Labels.TotalHours,
                Labels.LateIn,
                Labels.EarlyOut,
                Labels.Note
            };
            var rowIndex = 3;
            IRow row = sheet.CreateRow(rowIndex);
            RenderHeaderExportEmployeeAttendance(workbook, row, titleHeaders);

            // body
            RenderBodyExportEmployeeAttendance(listEmployeeAttendances, workbook, sheet, row);

            //// Auto-size each column
            for (var i = 0; i < sheet.GetRow(3).LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }
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
                cellBody.SetCellValue(employee.DateOfBirth.ToString("dd-MMM-yyyy"));

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

        private void RenderHeaderExportEmployeeAttendance(HSSFWorkbook workbook, IRow row, List<string> titleHeaders)
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

            for (int i = 0; i < titleHeaders.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(titleHeaders[i]);
            }
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void RenderBodyExportEmployeeAttendance(
            IList<EmployeeAttendanceModel> listEmployeeAttendances,
            HSSFWorkbook workbook,
            ISheet sheet,
            IRow row)
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

            // Add data rows.
            foreach (var employeeAttendance in listEmployeeAttendances)
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
                cellBody.SetCellValue(FormatDate(employeeAttendance.AttendanceDate));

                cellBody = row.CreateCell(2);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employeeAttendance.Code);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle4;
                }
                else
                {
                    bodyStyle = bodyStyle3;
                }

                cellBody = row.CreateCell(3);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employeeAttendance.NickName);

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employeeAttendance.FullName);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employeeAttendance.Team);

                cellBody = row.CreateCell(6);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employeeAttendance.ShiftCode);

                cellBody = row.CreateCell(7);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(GetTime(employeeAttendance.InFixed));

                cellBody = row.CreateCell(8);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(GetTime(employeeAttendance.OutFixed));

                cellBody = row.CreateCell(9);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(TotalHoursToTime(employeeAttendance.TotalWorkHours));

                cellBody = row.CreateCell(10);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(DisplayMinute(employeeAttendance.LateIn));

                cellBody = row.CreateCell(11);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(DisplayMinute(employeeAttendance.EarlyOut));

                cellBody = row.CreateCell(12);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(employeeAttendance.Notes);

                rowIndex++;
            }
        }

        private string FormatDate(DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }

            return date.Value.ToString("dd MMM, yyyy");
        }

        private string GetTime(DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }

            return date.Value.ToString("hh:mm");
        }

        private string DisplayMinute(int minutes)
        {
            if (minutes > 0)
            {
                var hour = Math.Floor(Convert.ToDouble(minutes) / 60);
                var minute = minutes % 60;
                if (hour == 0)
                {
                    return minute > 9 ? "00:" + minute : "00:0" + minute;
                }

                return (hour > 9 ? hour.ToString() : "0" + hour) + ':' + (minute > 9 ? minute.ToString() : "0" + minute);
            }

            return string.Empty;
        }

        private string TotalHoursToTime(decimal value)
        {
            if (value > 0)
            {
                var display = string.Empty;
                var hour = Math.Floor(value);
                if (hour > 0)
                {
                    display = hour < 10 ? "0" + hour + "h" : hour + "h";
                }

                var minute = Math.Round((value - hour) * 60);
                return minute < 10 ? display + "0" + minute + "'" : display + minute + "'";
            }

            return string.Empty;
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