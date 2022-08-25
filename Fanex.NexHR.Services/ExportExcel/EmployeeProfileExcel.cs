namespace Fanex.NexHR.Services.ExportExcel
{
    using System.Collections.Generic;
    using System.Drawing;
    using Fanex.NexHR.Models;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public class EmployeeProfileExcel : ExcelBase
    {
        private readonly IList<Employee> employee;
        private const byte RowStartHeader = 3;
        private const byte RowStartBody = 4;
        private const byte TotalCols = 29;
        private int rowIndex = RowStartBody;
        private int rowNumber = 1;

        public EmployeeProfileExcel(string templatePath, IList<Employee> employee) 
            : base(templatePath, 0, "EmployeeProfile")
        {
            this.employee = employee;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1, RowStartHeader - 1, TotalCols]
                   .Copy(worksheet.Cells[1, 1, RowStartHeader - 1, TotalCols]);
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]);
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            foreach (var emp in employee)
            {
                RenderRow(emp, worksheet);
                rowIndex++;
                rowNumber++;
            }
        }

        protected internal virtual void RenderRow(Employee employee, ExcelWorksheet worksheet)
        {
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = rowNumber;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.DateOfBirth.ToString("MM/dd/yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = employee.PlaceOfBirth;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Email;
            worksheet.Cells[rowIndex, colIndex++].Value = JobTitle(employee);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatString(employee.Team.Name);
            worksheet.Cells[rowIndex, colIndex++].Value = employee.JoinedDate?.ToString("MM/dd/yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ConfirmedDate?.ToString("MM/dd/yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ResignedDate?.ToString("MM/dd/yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = FormatString(employee.Phone);
            worksheet.Cells[rowIndex, colIndex++].Value = Gender(employee);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatString(employee.CardNo);
            worksheet.Cells[rowIndex, colIndex++].Value = employee.IssuedDate?.ToString("MM/dd/yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = FormatString(employee.IssuedPlace);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatString(employee.CurrentAddress);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatString(employee.PermanentAddress);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatString(employee.PassportNo);
            worksheet.Cells[rowIndex, colIndex++].Value = employee.PassportExp?.ToString("MM/dd/yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = FormatToeicScore(employee.ToeicScore?.Listening);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatToeicScore(employee.ToeicScore?.Reading);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatToeicScore(employee.ToeicScore?.Speaking);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatToeicScore(employee.ToeicScore?.Writing);
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ContactName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.UserRelationship;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ContactPhone;
            worksheet.Cells[rowIndex, colIndex].Value = employee.ContactAddress;

            if ((rowIndex & 1) == 0)
            {
                SetBackgroundColor(worksheet.Cells[rowIndex, 1, rowIndex, colIndex], ColorTranslator.FromHtml("#C6C8FF"));
            }
        }

        private static string JobTitle(Employee employee)
        {
            return employee.JobTitle == null ? string.Empty : employee.JobTitle.Name;
        }

        private static string Gender(Employee employee)
        {
            return employee.Gender == 0 ? "F" : "M";
        }

        private static string FormatString(string value)
        {
            return value?.Trim() ?? string.Empty;
        }

        private static dynamic FormatToeicScore(int? score)
        {
            if (score == null)
            {
                return string.Empty;
            }

            return score.Value;
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            SetStyle(worksheet);
            SetWidthColumn(worksheet);
        }

        protected internal virtual void SetStyle(ExcelWorksheet worksheet)
        {
            int[] colWithCenter = new[] { 1, 2, 4, 6, 10, 11, 12, 13, 14, 15 , 16, 17, 20, 21, 22, 23, 24, 25, 27, 28 };
            int[] colWithLeft = new[] { 3, 5, 7, 8, 9, 18, 19, 26, 29 };
            foreach(int index in colWithCenter) {
                worksheet.Column(index).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            foreach(int index in colWithLeft) {
                worksheet.Column(index).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[RowStartHeader, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            for (var index = 0; index < employee.Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int RowHeightTitle = 25;
            worksheet.Row(1).Height = RowHeightTitle;

            int[] widthCol = new[] { 5, 10, 30, 15, 20, 15, 25, 30, 25, 25, 25, 25, 30, 15, 17, 15, 15, 70, 70, 15, 15, 15, 15, 15, 15, 30, 15, 25, 70 };
            for (int i = 0; i < widthCol.Length; i++)
            {
                worksheet.Column(i + 1).Width = widthCol[i];
            }
        }
    }
}