namespace Fanex.NexHR.Services.ExportExcel
{
    using Fanex.NexHR.Models.Applicant;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class ApplicantsReportExcel : ExcelBase
    {
        private readonly IEnumerable<Applicant> applicants;
        private readonly DateTime? fromDate;
        private readonly DateTime? toDate;
        private readonly int pageNumber;
        private readonly int pageSize;
        private const byte RowStartHeader = 3;
        private const byte RowStartBody = 5;
        private const byte TotalCols = 13;
        private int rowIndex = RowStartBody;
        private const byte FontSizeTitle = 16;
        private int rowNumber;

        public ApplicantsReportExcel(string templatePath, IEnumerable<Applicant> applicants, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
            : base(templatePath, 0, "Applicants")
        {
            this.applicants = applicants;
            this.fromDate = fromDate;
            this.toDate = toDate;
            this.pageNumber = pageNumber;
            this.pageSize =pageSize;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1, 1, TotalCols]
                   .Copy(worksheet.Cells[1, 1, 1, TotalCols]);

            const string formatDate = "dd MMM, yyyy";
            worksheet.Cells[1, 1].Value = "Applicants: " + fromDate?.ToString(formatDate) + " - " + toDate?.ToString(formatDate);
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1].Style.Font.Size = FontSizeTitle;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader - 1, 1, RowStartHeader, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartHeader + 1, TotalCols]);
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            const int rowFreeze = 5;
            rowNumber = (pageNumber - 1) * pageSize + 1;

            foreach (var applicant in applicants)
            {
                RenderRow(applicant, worksheet);
                rowIndex++;
                rowNumber++;
            }
            worksheet.View.FreezePanes(rowFreeze, 1);
        }

        protected internal virtual void RenderRow(Applicant applicant, ExcelWorksheet worksheet)
        {
            const string formatDate = "dd MMM, yyyy";
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = rowNumber;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.Phone;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.Email;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.Position;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.InterviewDate1?.ToString(formatDate);
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.Result1;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.InterviewDate2?.ToString(formatDate);
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.Result2;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.Status;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.JoinDate?.ToString(formatDate);
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.Note;
            worksheet.Cells[rowIndex, colIndex++].Value = applicant.Source;

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
            const int columnPhone = 3;
            const int columnNote = 12;
            worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(columnPhone).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[RowStartHeader, TotalCols].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            for (int index = 6; index <= TotalCols; index++)
            {
                worksheet.Column(index).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            worksheet.Column(columnNote).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            
            worksheet.Cells[columnPhone, columnNote].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            for (var index = 0; index < applicants.ToList().Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int RowHeightTitle = 25;
            worksheet.Row(1).Height = RowHeightTitle;

            int[] widthCol = [10, 30, 15, 25, 30, 15, 15, 15, 15, 15, 15, 40, 15];
            for (int i = 0; i < widthCol.Length; i++)
            {
                worksheet.Column(i + 1).Width = widthCol[i];
            }
        }
    }
}