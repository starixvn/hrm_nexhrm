namespace Fanex.NexHR.Services.ExportExcel
{
    using Fanex.NexHR.Models;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class TotalWorkingHoursExcel : ExcelBase
    {
        private readonly IList<TotalWorkingHours> totalWorkingHours;
        private readonly DateTime fromDate;
        private readonly DateTime toDate;
        private const byte RowStartTitle = 2;
        private const byte RowStartHeader = 4;
        private const byte RowStartBody = 6;
        private const byte TotalCols = 16;
        private const byte FontSizeTitle = 16;
        private const byte RowDeductHeader = 2;
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
            worksheet.Cells[1, 1].Style.Font.Size = FontSizeTitle;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader - 1, 1, RowStartBody - RowDeductHeader, TotalCols]
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
            worksheet.Cells[rowIndex, colIndex++].Value = rowNumber;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.Employee.Code.Trim();
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.Employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.Employee.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.Employee.Team.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.WorkingDay;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.ActualWorkingDay;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.AnnualLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.PaidLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.SickLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.InsuranceLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.UnpaidLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.LateIn;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.EarlyOut;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.TotalHours;
            worksheet.Cells[rowIndex, colIndex++].Value = totalWorking.AverageHours;

            if ((rowIndex & 1) == 0)
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
            const int columnTwo = 2;
            const int columnThree = 3;
            worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(columnTwo).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(columnThree).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[RowStartHeader, columnThree].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            for (int index = 4; index <= TotalCols; index++)
            {
                worksheet.Column(index).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            for (var index = 0; index < totalWorkingHours.Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int RowHeightTitle = 25;
            worksheet.Row(1).Height = RowHeightTitle;

            int[] widthCol = new[] { 5, 15, 30, 15, 15, 15, 20, 15, 15, 15, 15, 15, 15, 15, 15, 15 };
            for (int i = 0; i < widthCol.Length; i++)
            {
                worksheet.Column(i + 1).Width = widthCol[i];
            }
        }
    }
}