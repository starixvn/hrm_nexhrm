namespace Fanex.NexHR.Core.NPOIExtensions
{
    using NPOI.SS.UserModel;
    using System;

    public static class HSSFRowExtensions
    {
        public static ICell CreateCell(this IRow row, int colIndex, ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(colIndex);

            cell.CellStyle = cellStyle;

            return cell;
        }

        public static void WriteCells(this IRow row, int colIndex, object[] values)
        {
            int id = colIndex;
            foreach (var value in values)
            {
                if (value is DateTime)
                {
                    var convert = Convert.ToDateTime(value);
                    var formatValue = convert.ToString("d MMM, yyyy");
                    row.GetCell(id).SetCellValue(formatValue);
                }
                else
                {
                    row.GetCell(id).SetCellValue(value);
                }

                id++;
            }
        }
    }
}