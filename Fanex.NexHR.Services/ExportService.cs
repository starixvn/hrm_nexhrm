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
    }
}