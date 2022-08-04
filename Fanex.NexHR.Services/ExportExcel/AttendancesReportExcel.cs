using Fanex.NexHR.Models.Report;
using OfficeOpenXml;
using System.Collections.Generic;

namespace Fanex.NexHR.Services.ExportExcel
{
    public class AttendancesReportExcel : ExcelBase
    {
        private readonly IList<EmployeeAttendanceModel> employeeAttendanceModel;
        private const byte RowStartHeader = 3;
        private const byte RowStartBody = 4;
        private const byte TotalCols = 14;

        public AttendancesReportExcel(string templatePath, IList<EmployeeAttendanceModel> EmployeeAttendanceModel) 
            : base(templatePath, 0, "EmployeeAttendance")
        {
            employeeAttendanceModel = EmployeeAttendanceModel;
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
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
    }
}
