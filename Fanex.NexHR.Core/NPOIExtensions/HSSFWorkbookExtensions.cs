// ----------------------------------------------------------------------
// <copyright file="HSSFWorkbookExtensions.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) .. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.27.01">Created.</change>
// </history>

namespace Fanex.NexHR.Core.NPOIExtensions
{
    using System.Web;
    using NPOI.SS.UserModel;

    public static class HSSFWorkbookExtensions
    {
        public static ISheet CloneSheet(this IWorkbook workbook, string sourceSheetName, string newSheetName)
        {
            var sheetIndex = workbook.GetSheetIndex(sourceSheetName);
            var newSheet = workbook.CloneSheet(sheetIndex);
            var newSheetIndex = workbook.GetSheetIndex(newSheet);
            workbook.SetSheetName(newSheetIndex, newSheetName);
            return newSheet;
        }

        public static void ToReponseStream(this IWorkbook workbook, string fileName)
        {
            var response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", $"attachment; filename={fileName}.xls");

            workbook.Write(response.OutputStream);
        }
    }
}