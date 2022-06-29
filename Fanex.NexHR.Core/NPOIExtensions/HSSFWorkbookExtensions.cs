namespace Fanex.NexHR.Core.NPOIExtensions
{
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

    }
}