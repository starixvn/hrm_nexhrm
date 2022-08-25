namespace Fanex.NexHR.Services.ExportExcel
{
    using System.Collections.Generic;
    using System.Drawing;
    using Fanex.NexHR.Models.Calendar;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public class LeaveScheduleExcel : ExcelBase
    {
        private readonly LeaveDaySummarySchedule schedules;
        private const byte RowStartHeader = 2;
        private const byte RowStartBody = 5;
        private const byte TotalCols = 35;
        private const byte RowSum = 6;
        private int rowIndex = RowStartBody;
        private int rowNumber = 1;

        public LeaveScheduleExcel(string templatePath, LeaveDaySummarySchedule schedules) 
            : base(templatePath, 0, "DayOffSchedule")
        {
            this.schedules = schedules;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1, 1, TotalCols]
                   .Copy(worksheet.Cells[1, 1, 1, TotalCols]);
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader, 1, RowStartBody - 1, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartBody - 1, TotalCols]);
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            RenderLeaveDaysSchedule(schedules, worksheet);

            foreach (var empSchedule in schedules.EmployeeLeaveShedules)
            {
                RenderRow(empSchedule, worksheet);
                rowIndex++;
                rowNumber++;
            }

            RenderLeaveDayScheduleSumRow(schedules.TotalLeaves, templateWorksheet, worksheet, rowIndex);

        }

        private static void RenderLeaveDaysSchedule(LeaveDaySummarySchedule schedules, ExcelWorksheet worksheet)
        {
            var startCol = 5;

            worksheet.Cells[RowStartHeader, RowStartBody, RowStartHeader, TotalCols].Value = schedules.Month.ToString("MMM yyyy");
            foreach (var date in schedules.ListDates)
            {
                worksheet.Cells[RowStartBody - 1, startCol].Value = date.Date.ToString("ddd");
                startCol++;
            }
        }

        private static void RenderLeaveDayScheduleSumRow(IList<double> totalLeaves, ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, int rowIndex)
        {
            var startCol = 5;

            templateWorksheet.Cells[RowSum, 1, RowSum , TotalCols]
                .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);

            foreach (var totalLeave in totalLeaves)
            {
                worksheet.Cells[rowIndex, startCol].Value = totalLeave;
                worksheet.Cells[rowIndex, startCol++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }

        protected internal virtual void RenderRow(EmployeeLeaveSchedule empSchedule, ExcelWorksheet worksheet)
        {
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = rowNumber;
            worksheet.Cells[rowIndex, colIndex++].Value = empSchedule.Employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = empSchedule.Employee.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = empSchedule.Employee.CodeOfTeam;

            foreach (var leaveSchedule in empSchedule.LeaveSchedules)
            {
                const string value = "x";
                var cellValue = leaveSchedule.LeaveType;

                if (leaveSchedule.IsLeaveDay)
                {
                    cellValue = !leaveSchedule.IsHalfDayLeave ? value : leaveSchedule.LeaveType;
                }
                worksheet.Cells[rowIndex, colIndex].Value = cellValue;
                worksheet.Cells[rowIndex, colIndex++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

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
            worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[RowStartHeader, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            for (int index = 3; index <= TotalCols; index++)
            {
                worksheet.Column(index).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            for (var index = 0; index < schedules.EmployeeLeaveShedules.Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int RowHeightTitle = 25;
            worksheet.Row(1).Height = RowHeightTitle;

            int[] widthCol = new[] {
                5, 30, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
                15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15
            };
            for (int i = 0; i < widthCol.Length; i++)
            {
                worksheet.Column(i + 1).Width = widthCol[i];
            }
        }
    }
}