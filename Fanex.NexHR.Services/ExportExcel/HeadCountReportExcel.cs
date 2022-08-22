using Fanex.NexHR.Models.Report;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Linq;
using Fanex.NexHR.Resources;

namespace Fanex.NexHR.Services.ExportExcel
{
    public class HeadCountReportExcel : ExcelBase
    {
        private readonly HeadCountModel headCountModel;
        private const byte RowStartHeader = 2;
        private const byte RowStartBody = 3;
        private const byte RowSum = 4;
        private const byte TotalCols = 10;
        private int rowIndex = RowStartBody;
        private int rowNumber = 1;

        public HeadCountReportExcel(string templatePath, HeadCountModel headCountModel, int year) 
            : base(templatePath, 0, "HeadCount" + year)
        {
            this.headCountModel = headCountModel;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1, RowStartHeader - 1, TotalCols]
                   .Copy(worksheet.Cells[1, 1, RowStartHeader - 1, TotalCols]);
            worksheet.Cells[1, 1].Value = string.Format("{0}{1}", "HEAD COUNT ", headCountModel.StartDate.Year);
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]
                .Copy(worksheet.Cells[RowStartHeader, 1, RowStartHeader, TotalCols]);
            
            worksheet.Cells[2, 4].Value = headCountModel.StartDate.ToString("dd MMM yyyy");
            worksheet.Cells[2, 9].Value = headCountModel.EndDate.ToString("dd MMM yyyy");
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            foreach (var headCount in headCountModel.HeadCount)
            {
                RenderRow(headCount, worksheet);
                rowIndex++;
                rowNumber++;
            }
            RenderSumRow(headCountModel.HeadCount, templateWorksheet, worksheet, rowIndex);
        }

        protected internal virtual void RenderRow(HeadCount headCount, ExcelWorksheet worksheet)
        {
            int colIndex = 1;
            worksheet.Cells[rowIndex, colIndex++].Value = rowNumber;
            worksheet.Cells[rowIndex, colIndex++].Value = headCount.GroupName;
            worksheet.Cells[rowIndex, colIndex++].Value = headCount.GroupCode;
            worksheet.Cells[rowIndex, colIndex++].Value = headCount.EmployeeAtJanuary;
            worksheet.Cells[rowIndex, colIndex++].Value = headCount.MoveTeam != 0 ? (-headCount.MoveTeam).ToString() : string.Empty;
            worksheet.Cells[rowIndex, colIndex++].Value = headCount.JoinTeam != 0 ? ("+" + headCount.JoinTeam) : string.Empty;
            worksheet.Cells[rowIndex, colIndex++].Value = headCount.NewHire != 0 ? headCount.NewHire.ToString() : string.Empty;
            worksheet.Cells[rowIndex, colIndex++].Value = headCount.Resigned != 0 ? headCount.Resigned.ToString() : string.Empty;
            worksheet.Cells[rowIndex, colIndex++].Value = headCount.EmployeeAtDecember;
            worksheet.Cells[rowIndex, colIndex].Value = headCount.Variance;
        }

        private static void RenderSumRow(IList<HeadCount> headCount, ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, int rowIndex)
        {
            var startCol = 4;

            templateWorksheet.Cells[RowSum, 1, RowSum , TotalCols]
                .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalCols]);
            worksheet.Cells[rowIndex, 1, rowIndex, 3].Merge = true;
            worksheet.Cells[rowIndex, 1].Value = Labels.Total;
            worksheet.Cells[rowIndex, startCol++].Value = headCount.Sum(x => x.EmployeeAtJanuary);
            worksheet.Cells[rowIndex, startCol++].Value = headCount.Sum(x => x.MoveTeam);
            worksheet.Cells[rowIndex, startCol++].Value = headCount.Sum(x => x.JoinTeam);
            worksheet.Cells[rowIndex, startCol++].Value = headCount.Sum(x => x.NewHire);
            worksheet.Cells[rowIndex, startCol++].Value = headCount.Sum(x => x.Resigned);
            worksheet.Cells[rowIndex, startCol++].Value = headCount.Sum(x => x.EmployeeAtDecember);
            worksheet.Cells[rowIndex, startCol].Value = headCount.Sum(x => x.Variance);
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
            for (var index = 0; index < headCountModel.HeadCount.Count; index++)
            {
                DrawBorderCell(worksheet.Cells[RowStartBody + index, 1, RowStartBody + index, TotalCols]);
            }
        }

        protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
        {
            const int WidthSmall = 5;
            const int WidthNormal = 12;
            const int RowHeightTitle = 25;
            const int WidthFull = 30;

            const int colIndex = 1;
            const int colTeam = 2;

            worksheet.Row(1).Height = RowHeightTitle;
            worksheet.DefaultColWidth = WidthNormal;

            worksheet.Column(colIndex).Width = WidthSmall;
            worksheet.Column(colTeam).Width = WidthFull;
        }
    }
}