using Fanex.NexHR.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace Fanex.NexHR.Services.ExportExcel
{
    public class TotalWorkingHoursExcel : ExcelBase
    {
        private readonly IList<TotalWorkingHours> totalWorkingHours;
        private readonly DateTime fromDate;
        private readonly DateTime toDate;
        private const byte RowStartTitle = 2;
        private const byte RowStartHeader = 4;
        private const byte RowStartBody = 6;
        private const byte TotalCols = 16;
        private int rowIndex = RowStartBody;
        private int rowNumber = 1;

        public TotalWorkingHoursExcel(string templatePath, IList<TotalWorkingHours> totalWorkingHours, DateTime fromDate, DateTime toDate) 
            : base(templatePath, 0, "TotalWorkingHours")
        {
            this.totalWorkingHours = totalWorkingHours;
            this.fromDate = fromDate;
            this.toDate = toDate;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1, 1, TotalCols]
                   .Copy(worksheet.Cells[RowStartTitle, 1, RowStartTitle, TotalCols]);

            const string formatDate = "dd MMM, yyyy";

            worksheet.Cells[1, 1, 1, TotalCols].Merge = true;
            worksheet.Cells[1, 1].Value = "Total Working Hours: " + fromDate.ToString(formatDate) + " - " + toDate.ToString(formatDate);
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader - 1, 1, RowStartBody - 2, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartBody - 1, TotalCols]);
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            foreach (var totalWorking in totalWorkingHours)
            {
                RenderRow(totalWorking, worksheet);
                rowIndex++;
                rowNumber++;
            }
        }

        protected internal virtual void RenderRow(TotalWorkingHours totalWorking, ExcelWorksheet worksheet)
        {
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex].Value = rowNumber;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.Employee.Code.Trim();
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.Employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.Employee.NickName;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.Employee.Team.Code;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.WorkingDay;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.ActualWorkingDay;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.AnnualLeave;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.PaidLeave;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.SickLeave;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.InsuranceLeave;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.UnpaidLeave;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.LateIn;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.EarlyOut;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.TotalHours;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[rowIndex, colIndex].Value = totalWorking.AverageHours;
            worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if ((rowIndex & 1) == 0)
            {
                SetBackgroundColor(worksheet.Cells[rowIndex, 1, rowIndex, colIndex - 1], ColorTranslator.FromHtml("#C6C8FF"));
            }
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            SetWidthColumn(worksheet);
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int WidthSmall = 5;
            const int WidthNormal = 15;
            const int WidthActualWorking = 20;
            const int RowHeightTitle = 25;
            const int WidthFull = 30;

            const int colIndex = 1;
            const int colFullName = 3;
            const int colActualWorking = 7;

            worksheet.Row(1).Height = RowHeightTitle;
            worksheet.DefaultColWidth = WidthNormal;
            worksheet.Cells.AutoFitColumns();

            worksheet.Column(colIndex).Width = WidthSmall;
            worksheet.Column(colFullName).Width = WidthFull;
            worksheet.Column(colActualWorking).Width = WidthActualWorking;
        }
    }
}