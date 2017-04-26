// ----------------------------------------------------------------------
// <copyright file="ExcelTemplateWriter.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) .. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.27.01">Created.</change>
// </history>

namespace Fanex.NexHR.Core
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOIExtensions;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class ExcelTemplateWriter
    {
        #region Attributes

        private readonly string _fileName;
        private readonly ISheet _templateSheet;
        private readonly ISheet _sheet;
        private int _rowId;

        #endregion Attributes

        #region Constructors

        public ExcelTemplateWriter(FileStream fs, string excelFileName = "Report.xls", string sheetName = "Result")
            : this(new HSSFWorkbook(fs), excelFileName, sheetName)
        {
            fs.Close();
        }

        public ExcelTemplateWriter(HSSFWorkbook template, string excelFileName, string sheetName)
        {
            _fileName = excelFileName;
            _templateSheet = template.GetSheetAt(0);
            _sheet = template.CreateSheet(sheetName);
        }

        #endregion Constructors

        #region Properties

        public int RowId => _rowId;

        #endregion Properties

        public void Render()
        {
            _sheet.Workbook.RemoveSheetAt(0);
            _sheet.Workbook.ToReponseStream(_fileName);
        }

        public void WriteCellRange(int rowIndex, int totalRow, int colIndex, int totalCol, object value)
        {
            var cellRange = new CellRangeAddress(rowIndex, rowIndex + totalRow - 1, colIndex, colIndex + totalCol - 1);
            _sheet.AddMergedRegion(cellRange);

            var cell = _sheet.GetCell(rowIndex, colIndex);
            cell.SetCellValue(value);
        }

        #region Auto resize columns' width

        public void AutosizeColumns(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                _sheet.AutoSizeColumn(i, false);
            }
        }

        public void AutosizeColumns(int total)
        {
            AutosizeColumns(0, total - 1);
        }

        #endregion Auto resize columns' width

        #region Synchronize columns' width

        public void SyncColumnsWidth(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                _sheet.SetColumnWidth(i, _templateSheet.GetColumnWidth(i));
            }
        }

        public void SyncColumnsWidth(int total)
        {
            SyncColumnsWidth(0, total - 1);
        }

        #endregion Synchronize columns' width

        #region Row related methods

        public void WriteRow(int rowIndex, object[] values)
        {
            IRow row = CreateRow(rowIndex);
            row.WriteCells(0, values);
        }

        public void WriteRows(int rowIndex, List<object[]> arrayList)
        {
            int total = arrayList.Count;
            IRow row = CreateRows(rowIndex, total);
            for (int i = 0; i < total; i++)
            {
                row.WriteCells(0, arrayList[i]);
                row = _sheet.GetRow(row.RowNum + 1);
            }
        }

        public void WriteRow(int rowIndex, object value)
        {
            var list = new List<object>();

            foreach (PropertyInfo propertyInfo in value.GetType().GetProperties())
            {
                list.Add(propertyInfo.GetValue(value, null));
            }

            WriteRow(rowIndex, list.ToArray());
        }

        public void WriteRows(int rowId, IEnumerable<object> recordList)
        {
            foreach (var record in recordList)
            {
                WriteRow(rowId, record);
            }
        }

        public IRow CopyRows(int rowIndex, int total)
        {
            return CreateRows(rowIndex, total);
        }

        private IRow CreateRow(int rowId)
        {
            return CreateRows(rowId, 1);
        }

        private IRow CreateRows(int rowIndex, int total)
        {
            _sheet.CopyRows(_templateSheet, rowIndex, _rowId, total);
            _rowId += total;
            return _sheet.GetRow(_rowId - total);
        }

        #endregion Row related methods

        public void CreateFreezePane(int colSplit, int rowSplit)
        {
            _sheet.CreateFreezePane(colSplit, rowSplit);
        }
    }
}