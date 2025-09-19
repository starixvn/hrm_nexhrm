namespace Fanex.NexHR.Services.ExportExcel
{
    using Fanex.NexHR.Models.Report;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class LateInEarlyOutReportExcel(
        string templatePath,
        IEnumerable<LateInEarlyOutSummary> lateInEarlyOutSummaries,
        int year)
        : ExcelBase(templatePath, 0, "LateInEarlyOutReport")
    {
        private const byte RowStartHeader = 3;
        private const byte RowStartBody = 5;
        private const byte TotalCols = 19;
        private int rowIndex = RowStartBody;
        private int rowNumber = 1;

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            // Set title
            worksheet.Cells[1, 1].Value = $"LATE IN/EARLY OUT REPORT - {year}";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Merge title cells
            worksheet.Cells[1, 1, 1, TotalCols].Merge = true;

            // Add explanatory notes
            worksheet.Cells[2, 1].Value = "Deduction Taken (Hours): The grand total hour(s) of Late In/Early Out, resulted in a deduction taken from Leave Balance.";
            worksheet.Cells[2, 1].Style.Font.Size = 10;
            worksheet.Cells[2, 1].Style.Font.Italic = true;
            worksheet.Cells[2, 1, 2, TotalCols].Merge = true;
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            // Main header row
            worksheet.Cells[RowStartHeader, 1].Value = "English Name";
            worksheet.Cells[RowStartHeader, 2].Value = $"{year - 1} (Hours)";
            worksheet.Cells[RowStartHeader, 3].Value = $"{year} (Hours / Times)";
            worksheet.Cells[RowStartHeader, 16].Value = "Total(s)";
            worksheet.Cells[RowStartHeader, 17].Value = "Deduction Taken";
            worksheet.Cells[RowStartHeader, 18].Value = "Used NEX";
            worksheet.Cells[RowStartHeader, 19].Value = "Balance";

            // Merge the year column header across month columns
            worksheet.Cells[RowStartHeader, 3, RowStartHeader, 15].Merge = true;

            // Month headers - place them in the second row
            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            for (int i = 0; i < months.Length; i++)
            {
                worksheet.Cells[RowStartHeader + 1, 3 + i].Value = months[i];
            }

            // Style headers
            var headerRange = worksheet.Cells[RowStartHeader, 1, RowStartHeader + 1, TotalCols];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(HeaderTitleColor);
            headerRange.Style.Font.Color.SetColor(Color.White);
            DrawBorderCell(headerRange);
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            if (lateInEarlyOutSummaries == null || !lateInEarlyOutSummaries.Any())
            {
                return;
            }

            // Group by team
            var lateInEarlyOutByTeams = lateInEarlyOutSummaries.GroupBy(t => t.TeamId).Select(grp => grp.First()).ToList();

            foreach (var lateInEarlyOutByTeam in lateInEarlyOutByTeams)
            {
                // Add team header row
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Value = lateInEarlyOutByTeam.TeamName.ToUpperInvariant();
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Merge = true;
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Style.Font.Bold = true;
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Style.Font.Color.SetColor(Color.Black);
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowIndex, 1, rowIndex, TotalCols].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                DrawBorderCell(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);
                rowIndex++;

                // Add employees in this team
                var lateInEarlyOuts = lateInEarlyOutSummaries.Where(t => t.TeamId == lateInEarlyOutByTeam.TeamId).ToList();
                foreach (var lateInEarlyOut in lateInEarlyOuts)
                {
                    RenderRow(lateInEarlyOut, worksheet);
                    rowIndex++;
                    rowNumber++;
                }
            }
        }

        protected virtual void RenderRow(LateInEarlyOutSummary lateInEarlyOut, ExcelWorksheet worksheet)
        {
            int colIndex = 1;

            // English Name
            worksheet.Cells[rowIndex, colIndex++].Value = lateInEarlyOut.NickName;

            // Last year balance
            worksheet.Cells[rowIndex, colIndex++].Value = lateInEarlyOut.BalanceLastYear != 0 ? (lateInEarlyOut.BalanceLastYear / 60.0).ToString("0.0") : "-";

            // Monthly data (Hours/Times format)
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Jan, lateInEarlyOut.TimesInJan);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Feb, lateInEarlyOut.TimesInFeb);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Mar, lateInEarlyOut.TimesInMar);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Apr, lateInEarlyOut.TimesInApr);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.May, lateInEarlyOut.TimesInMay);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Jun, lateInEarlyOut.TimesInJun);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Jul, lateInEarlyOut.TimesInJul);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Aug, lateInEarlyOut.TimesInAug);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Sep, lateInEarlyOut.TimesInSep);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Oct, lateInEarlyOut.TimesInOct);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Nov, lateInEarlyOut.TimesInNov);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.Dec, lateInEarlyOut.TimesInDec);

            // Total
            worksheet.Cells[rowIndex, colIndex++].Value = FormatMonthlyData(lateInEarlyOut.TotalCurrentYear, lateInEarlyOut.TotalTimesCurrentYear);

            // Deduction Taken
            worksheet.Cells[rowIndex, colIndex++].Value = lateInEarlyOut.LeaveDeduction != 0 ? (lateInEarlyOut.LeaveDeduction / 60.0).ToString("0.0") : "-";

            // Used NEX
            worksheet.Cells[rowIndex, colIndex++].Value = lateInEarlyOut.OvertimeBalance != 0 ? (lateInEarlyOut.OvertimeBalance / 60.0).ToString("0.0") : "-";

            // Balance
            worksheet.Cells[rowIndex, colIndex++].Value = lateInEarlyOut.BalanceCurrentYear != 0 ? (lateInEarlyOut.BalanceCurrentYear / 60.0).ToString("0.0") : "-";

            SetBackground(rowIndex, worksheet, colIndex);
        }

        private string FormatMonthlyData(int minutes, int times)
        {
            if (minutes == 0)
                return "-";

            return $"{minutes / 60.0:F1}/{times}";
        }

        private void SetBackground(int rowIndex, ExcelWorksheet worksheet, int colIndex)
        {
            if ((rowIndex & 1) == 0)
            {
                SetBackgroundColor(worksheet.Cells[rowIndex, 1, rowIndex, colIndex], EvenRowColor);
            }
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            SetStyle(worksheet);
            SetWidthColumn(worksheet);
        }

        protected internal virtual void SetStyle(ExcelWorksheet worksheet)
        {
            // Center align data columns (not team headers)
            int[] colWithCenter = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            foreach (int index in colWithCenter)
            {
                worksheet.Column(index).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Draw borders for all data rows
            for (var index = 0; index < lateInEarlyOutSummaries.Count() + lateInEarlyOutSummaries.GroupBy(t => t.TeamId).Count(); index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int RowHeightTitle = 25;
            worksheet.Row(1).Height = RowHeightTitle;
            worksheet.Row(2).Height = 15;

            int[] widthCol = { 20, 15, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 15, 15, 15 };
            for (int i = 0; i < widthCol.Length; i++)
            {
                worksheet.Column(i + 1).Width = widthCol[i];
            }
        }
    }
}
