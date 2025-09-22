namespace Fanex.NexHR.Services.ExportExcel
{
    using Fanex.NexHR.Models.Overtime;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using System.Collections.Generic;
    using System.Drawing;

    public class OvertimeSummariesReportExcel : ExcelBase
    {
        private readonly IEnumerable<OvertimeSummary> _overtimeSummaries;
        private const byte RowStartHeader = 4;
        private const byte RowStartBody = 5;
        private int rowIndex = RowStartBody;
        private int orderNumber = 1;
        private readonly bool _isHR;
        private readonly int TotalCols;
        private readonly int PayrollColumnPosition = 5;
        private readonly int _year;

        public OvertimeSummariesReportExcel(string templatePath, IEnumerable<OvertimeSummary> overtimeSummaries, int year, bool isHR = false)
            : base(templatePath, 0, "NexSummary")
        {
            _overtimeSummaries = overtimeSummaries;
            _isHR = isHR;
            _year = year;
            TotalCols = isHR ? 23 : 22;
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            foreach (var report in _overtimeSummaries)
            {
                SetValueRow(report, worksheet);
                rowIndex++;
                orderNumber++;
            }
        }

        protected internal virtual async void SetValueRow(OvertimeSummary report, ExcelWorksheet worksheet)
        {
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = orderNumber;
            worksheet.Cells[rowIndex, colIndex++].Value = report.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = report.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = report.TeamCode;
            if (_isHR)
            {
                worksheet.Cells[rowIndex, colIndex++].Value = report.PayrollCompanyName;
            }
            worksheet.Cells[rowIndex, colIndex++].Value = report.BalanceLastYear;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Jan;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Feb;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Mar;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Apr;
            worksheet.Cells[rowIndex, colIndex++].Value = report.May;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Jun;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Jul;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Aug;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Sep;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Oct;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Nov;
            worksheet.Cells[rowIndex, colIndex++].Value = report.Dec;
            worksheet.Cells[rowIndex, colIndex++].Value = report.TotalHour;
            worksheet.Cells[rowIndex, colIndex++].Value = report.LeaveConversion;
            worksheet.Cells[rowIndex, colIndex++].Value = report.OvertimeDeduct;
            worksheet.Cells[rowIndex, colIndex++].Value = report.LateInEarlyOut;
            worksheet.Cells[rowIndex, colIndex++].Value = report.BalanceCurrentYear;

            if ((rowIndex & 1) != 0)
            {
                SetBackgroundColor(worksheet.Cells[rowIndex, 1, rowIndex, colIndex - 1], ColorTranslator.FromHtml("#C6C8FF"));
            }
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]);

            templateWorksheet.Cells[RowStartHeader - 1, 1, RowStartHeader, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader - 1, 1, RowStartHeader, TotalCols]);

            int previousYearColumn = PayrollColumnPosition;
            worksheet.Cells[RowStartHeader - 1, previousYearColumn].Value = _year - 1;

            int currentYearColumn = PayrollColumnPosition + 1;
            worksheet.Cells[RowStartHeader - 1, currentYearColumn].Value = _year;

            if (_isHR)
            {
                worksheet.InsertColumn(PayrollColumnPosition, 1);
                var headerCell = worksheet.Cells[RowStartHeader - 1, PayrollColumnPosition];
                worksheet.Cells[RowStartHeader - 1, PayrollColumnPosition, RowStartHeader, PayrollColumnPosition].Merge = true;
                headerCell.Value = "Payroll Company";
                headerCell.Style.Font.Name = "Arial";
                headerCell.Style.Font.Bold = true;
                headerCell.Style.Font.Color.SetColor(Color.White);
                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(51, 51, 51));
                headerCell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headerCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headerCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1, RowStartHeader - 1, TotalCols]
                   .Copy(worksheet.Cells[1, 1, RowStartHeader - 1, TotalCols]);
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            SetStyle(worksheet);
            SetWidthColumn(worksheet);
        }

        protected internal virtual void SetStyle(ExcelWorksheet worksheet)
        {
            for (int index = 1; index <= TotalCols; index++)
            {
                worksheet.Column(index).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            IList<OvertimeSummary> overtimeSummaries = (IList<OvertimeSummary>)_overtimeSummaries;
            for (var index = 0; index < overtimeSummaries.Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            if (_isHR)
            {
                worksheet.Column(PayrollColumnPosition).Width = 25;
            }
        }
    }
}