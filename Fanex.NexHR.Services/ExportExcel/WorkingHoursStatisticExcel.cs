using Fanex.NexHR.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace Fanex.NexHR.Services.ExportExcel
{
    public class WorkingHoursStatisticExcel : ExcelBase
    {
        private readonly IList<WorkingHoursStatistic> workingHoursStatistics;
        private readonly DateTime fromDate;
        private readonly DateTime toDate;
        private const byte RowStartHeader = 3;
        private const byte RowStartBody = 5;
        private const byte TotalCols = 20;
        private int rowIndex = RowStartBody;
        private int rowNumber = 1;

        public WorkingHoursStatisticExcel(string templatePath, IList<WorkingHoursStatistic> workingHoursStatistics, DateTime fromDate, DateTime toDate) 
            : base(templatePath, 0, "Total Working Hours " + fromDate.ToString("MMM, yyyy"))
        {
            this.workingHoursStatistics = workingHoursStatistics;
            this.fromDate = fromDate;
            this.toDate = toDate;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1, 1, TotalCols]
                   .Copy(worksheet.Cells[1, 1, 1, TotalCols]);

            const string formatDate = "dd MMM, yyyy";
            worksheet.Cells[1, 1].Value = "Total Working Hours: " + fromDate.ToString(formatDate) + " - " + toDate.ToString(formatDate);
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader - 1, 1, RowStartHeader, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartHeader + 1, TotalCols]);
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            foreach (var workingHoursStatistic in workingHoursStatistics)
            {
                RenderRow(workingHoursStatistic, worksheet);
                rowIndex++;
                rowNumber++;
            }
        }

        protected internal virtual void RenderRow(WorkingHoursStatistic workingHoursStatistic, ExcelWorksheet worksheet)
        {
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = rowNumber;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.Employee.Code.Trim();
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.Employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.Employee.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.Employee.Team.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.ActualWorkingDay;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.AnnualLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.PaidLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.SickLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.InsuranceLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.UnpaidLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.BusinessLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.WorkFromHome;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.TotalWorkingDay;
            worksheet.Cells[rowIndex, colIndex++].Value = 0;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.PreAnnualLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.PreMonthSickLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.PreMonthUnpaidLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.PreMonthBusinessLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = workingHoursStatistic.PreWorkFromHome;

            if ((rowIndex & 1) != 0)
            {
                SetBackgroundColor(worksheet.Cells[rowIndex, 1, rowIndex, colIndex - 1], ColorTranslator.FromHtml("#C6C8FF"));
            }
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            SetStyle(worksheet);
            SetWidthColumn(worksheet);
        }

        protected internal virtual void SetStyle(ExcelWorksheet worksheet)
        {
            worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[RowStartHeader, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            for (int index = 4; index <= TotalCols; index++)
            {
                worksheet.Column(index).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            for (var index = 0; index < workingHoursStatistics.Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int RowHeightTitle = 25;
            worksheet.Row(1).Height = RowHeightTitle;

            int[] widthCol = new[] { 5, 15, 30, 15, 15, 20, 15, 15, 15, 15, 15, 20, 15, 20, 15, 15, 15, 15, 15, 15, 15 };
            for (int i = 0; i < widthCol.Length; i++)
            {
                worksheet.Column(i + 1).Width = widthCol[i];
            }
        }
    }
}