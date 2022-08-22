using Fanex.NexHR.Models.Report;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Drawing;

namespace Fanex.NexHR.Services.ExportExcel
{
    public class AttendancesReportExcel : ExcelBase
    {
        private readonly IList<EmployeeAttendanceModel> employeeAttendanceModel;
        private const byte RowStartHeader = 3;
        private const byte RowStartBody = 4;
        private const byte TotalCols = 14;
        private int rowIndex = RowStartBody;
        private int stt = 1;

        public AttendancesReportExcel(string templatePath, IList<EmployeeAttendanceModel> EmployeeAttendanceModel) 
            : base(templatePath, 0, "EmployeeAttendance")
        {
            employeeAttendanceModel = EmployeeAttendanceModel;
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
        protected internal virtual void SetValueRow(EmployeeAttendanceModel report, ExcelWorksheet worksheet)
        {
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = stt;
            worksheet.Cells[rowIndex, colIndex++].Value = report.AttendanceDate?.ToString("dd/MM/yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = report.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = report.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = report.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = report.TeamCode;
            worksheet.Cells[rowIndex, colIndex++].Value = report.ShiftCode;
            worksheet.Cells[rowIndex, colIndex++].Value = report.InFixed?.ToString("hh:mm");
            worksheet.Cells[rowIndex, colIndex++].Value = report.OutFixed?.ToString("hh:mm");
            var totalWorkHour = ((int)report.TotalWorkHours);
            var totalWorkMinute = (report.TotalWorkHours - totalWorkHour) * 100;
            worksheet.Cells[rowIndex, colIndex++].Value = report.TotalWorkHours == 0 ? "" 
                : ($"{string.Format("{0:00}", totalWorkHour)}h:{string.Format("{0:00}", totalWorkMinute)}'");  
            worksheet.Cells[rowIndex, colIndex++].Value = report.LateIn == 0 ? "" : ($"00:{report.LateIn}");
            worksheet.Cells[rowIndex, colIndex++].Value = report.EarlyOut == 0 ? "" : ($"00:{report.EarlyOut}");
            worksheet.Cells[rowIndex, colIndex++].Value = report.IsWorkFromHome ? "Yes" : "No";
            worksheet.Cells[rowIndex, colIndex].Value = report.Notes;

            if ((rowIndex & 1) != 0)
            {
                SetBackgroundColor(worksheet.Cells[rowIndex, 1, rowIndex, colIndex], ColorTranslator.FromHtml("#C6C8FF"));
            }
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]);
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
            worksheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[RowStartHeader, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            for (var index = 0; index < employeeAttendanceModel.Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int RowHeightTitle = 25;
            worksheet.Row(1).Height = RowHeightTitle;
            int[] widthCol = new[] { 5, 15, 10, 13, 30, 13, 8, 12, 12, 12, 12, 12, 25, 30 };
            for (int i = 0; i < widthCol.Length; i++)
            {
                worksheet.Column(i + 1).Width = widthCol[i];
            }
        }
    }
}
