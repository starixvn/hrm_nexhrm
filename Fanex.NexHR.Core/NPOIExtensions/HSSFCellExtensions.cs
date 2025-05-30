namespace Fanex.NexHR.Core.NPOIExtensions;

using System;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

public static class HSSFCellExtensions
{
    public static void SetCellValue(this ICell cell, object value)
    {
        if (value == null)
        {
            cell.SetCellValue(string.Empty);
            return;
        }

        var stringValue = value as string;

        if (!string.IsNullOrEmpty(stringValue))
        {
            cell.SetCellValue(stringValue);
        }
        else
        {
            dynamic dynamicValue = value;
            cell.SetCellValue(dynamicValue);
        }
    }

    public static void SetCellValue(this ICell cell, double? value)
    {
        if (value != null)
        {
            cell.SetCellValue((double)value);
        }
        else
        {
            cell.SetCellValue(0);
        }
    }

    public static void SetCellValue(this ICell cell, decimal value)
    {
        cell.SetCellValue(Convert.ToDouble(value));
    }

    public static void SetCellValue(this ICell cell, decimal? value)
    {
        if (value != null)
        {
            cell.SetCellValue((decimal)value);
        }
        else
        {
            cell.SetCellValue(0);
        }
    }

    public static void SetCellValue(this ICell cell, int value)
    {
        cell.SetCellValue(Convert.ToDouble(value));
    }

    public static void SetCellValue(this ICell cell, int? value)
    {
        cell.SetCellValue(value ?? 0);
    }

    public static void SetCellValue(this ICell cell, long value)
    {
        cell.SetCellValue(Convert.ToDouble(value));
    }

    public static void SetCellValue(this ICell cell, long? value)
    {
        if (value != null)
        {
            cell.SetCellValue((long)value);
        }
        else
        {
            cell.SetCellValue(0);
        }
    }

    public static void CopyFrom(this ICell cell, ICell sourceCell)
    {
        // Copy merged cells.
        CellRangeAddress sourceRange = null;

        for (var i = 0; i < sourceCell.Sheet.NumMergedRegions && sourceRange == null; i++)
        {
            CellRangeAddress range = sourceCell.Sheet.GetMergedRegion(i);

            if ((range.FirstColumn <= sourceCell.ColumnIndex)
                && (range.LastColumn >= sourceCell.ColumnIndex)
                && (range.FirstRow <= sourceCell.RowIndex)
                && (range.LastRow >= sourceCell.RowIndex))
            {
                sourceRange = range;
            }
        }

        if (sourceRange != null)
        {
            cell.Sheet.AddMergedRegion(sourceRange.Clone());
        }

        // Copy style
        cell.CellStyle = sourceCell.CellStyle;
    }
}