namespace Fanex.NexHR.Services.ExportExcel
{
    using Fanex.NexHR.Models.Report;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class AttendancesReportExcel : ExcelBase
    {
        private readonly IList<EmployeeAttendanceModel> employeeAttendanceModel;
        private const byte RowStartHeader = 3;
        private const byte RowStartBody = 4;
        private readonly int TotalCols;
        private const byte Five = 5;
        private int rowIndex = RowStartBody;
        private int stt = 1;
        private readonly bool _isHR;
        private readonly int PayrollColumnPosition = 8;

        public AttendancesReportExcel(string templatePath, IList<EmployeeAttendanceModel> EmployeeAttendanceModel, bool isHR = false)
            : base(templatePath, 0, "EmployeeAttendance")
        {
            employeeAttendanceModel = EmployeeAttendanceModel;
            _isHR = isHR;
            TotalCols = isHR ? 16 : 15;
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            foreach (var report in employeeAttendanceModel)
            {
                SetValueRow(report, worksheet);
                rowIndex++;
                stt++;
            }
        }

        protected internal virtual async void SetValueRow(EmployeeAttendanceModel report, ExcelWorksheet worksheet)
        {
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = stt;
            worksheet.Cells[rowIndex, colIndex++].Value = report.AttendanceDate?.ToString("dd/MM/yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = report.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = report.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = report.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = report.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = report.TeamCode;
            if (_isHR)
            {
                worksheet.Cells[rowIndex, colIndex++].Value = report.PayrollCompanyName;
            }
            worksheet.Cells[rowIndex, colIndex++].Value = report.ShiftCode;
            worksheet.Cells[rowIndex, colIndex++].Value = report.InFixed?.ToString("hh:mm");
            worksheet.Cells[rowIndex, colIndex++].Value = report.OutFixed?.ToString("hh:mm");

            worksheet.Cells[rowIndex, colIndex++].Value = report.TotalWorkHours == 0 ? ""
                : DisplayTotalWorkingHour(report.TotalWorkHours);
            worksheet.Cells[rowIndex, colIndex++].Value = report.LateIn == 0 ? "" : ($"00:{report.LateIn}");
            worksheet.Cells[rowIndex, colIndex++].Value = report.EarlyOut == 0 ? "" : ($"00:{report.EarlyOut}");
            worksheet.Cells[rowIndex, colIndex++].Value = report.IsWorkFromHome ? "Yes" : "No";
            worksheet.Cells[rowIndex, colIndex].Value = report.Notes;

            if ((rowIndex & 1) != 0)
            {
                SetBackgroundColor(worksheet.Cells[rowIndex, 1, rowIndex, colIndex], ColorTranslator.FromHtml("#C6C8FF"));
            }
        }

        private static string DisplayTotalWorkingHour(decimal totalWorkHour)
        {
            var display = string.Empty;
            var hour = Math.Floor(totalWorkHour);

            if (hour > 0)
            {
                display = string.Format("{0}h", hour < 10 ? "0" + hour : hour);
            }

            var minute = Math.Round((totalWorkHour - hour) * 60);

            return string.Format("{0}{1}'", display, minute < 10 ? "0" + minute : minute);
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]);

            if (_isHR)
            {
                worksheet.InsertColumn(PayrollColumnPosition, 1);
                var headerCell = worksheet.Cells[RowStartHeader, PayrollColumnPosition];
                headerCell.Value = "Payroll Company";
                headerCell.Style.Font.Name = "Arial";
                headerCell.Style.Font.Bold = true;
                headerCell.Style.Font.Color.SetColor(Color.White);
                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(51, 51, 51));
                headerCell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
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
            worksheet.Column(Five).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[RowStartHeader, Five].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            for (var index = 0; index < employeeAttendanceModel.Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }
    }
}