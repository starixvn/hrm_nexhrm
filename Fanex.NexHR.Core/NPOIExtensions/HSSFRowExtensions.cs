// ----------------------------------------------------------------------
// <copyright file="HSSFRowExtensions.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) .. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.27.01">Created.</change>
// </history>

namespace Fanex.NexHR.Core.NPOIExtensions
{
    using NPOI.HSSF.UserModel;
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
            var dateFormat = row.Sheet.Workbook.CreateDataFormat().GetFormat("d MMM, yyyy");

            foreach (var value in values)
            {
                row.GetCell(id).SetCellValue(value);

                if (value is DateTime)
                {
                    row.GetCell(id).CellStyle.CloneStyleFrom(row.GetCell(id).CellStyle);
                    row.GetCell(id).CellStyle.DataFormat = dateFormat;
                }

                id++;
            }
        }
    }
}