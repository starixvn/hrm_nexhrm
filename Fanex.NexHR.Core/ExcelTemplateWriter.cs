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
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Fanex.NexHR.Core.NPOIExtensions;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public class ExcelTemplateWriter
    {
        #region Attributes

        private string _fileName;
        private string _sheetName;
        private ISheet _templateSheet;
        private ISheet _sheet;
        private int _rowId = 0;

        #endregion Attributes

        #region Constructors

        public ExcelTemplateWriter(FileStream fs, string excelFileName = "Report.xls", string sheetName = "Result")
            : this(new HSSFWorkbook(fs), excelFileName, sheetName)
        {
            fs.Close();
        }

        public ExcelTemplateWriter(HSSFWorkbook template, string excelFileName, string sheetName)
        {
            this._fileName = excelFileName;
            this._sheetName = sheetName;
            this._templateSheet = template.GetSheetAt(0);
            this._sheet = template.CreateSheet(this._sheetName);
        }

        #endregion Constructors

        #region Properties

        public int RowId
        {
            get
            {
                return this._rowId;
            }
        }

        #endregion Properties

        public void Render()
        {
            this._sheet.Workbook.RemoveSheetAt(0);
            this._sheet.Workbook.ToReponseStream(this._fileName);
        }

        public void WriteCellRange(int rowIndex, int totalRow, int colIndex, int totalCol, object value)
        {
            var cellRange = new CellRangeAddress(rowIndex, rowIndex + totalRow - 1, colIndex, colIndex + totalCol - 1);
            this._sheet.AddMergedRegion(cellRange);

            var cell = this._sheet.GetCell(rowIndex, colIndex);
            cell.SetCellValue(value);
        }

        #region Auto resize columns' width

        public void AutosizeColumns(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                this._sheet.AutoSizeColumn(i, false);
            }
        }

        public void AutosizeColumns(int total)
        {
            this.AutosizeColumns(0, total - 1);
        }

        #endregion Auto resize columns' width

        #region Synchronize columns' width

        public void SyncColumnsWidth(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                this._sheet.SetColumnWidth(i, this._templateSheet.GetColumnWidth(i));
            }
        }

        public void SyncColumnsWidth(int total)
        {
            this.SyncColumnsWidth(0, total - 1);
        }

        #endregion Synchronize columns' width

        #region Row related methods

        public void WriteRow(int rowIndex, object[] values)
        {
            IRow row = this.CreateRow(rowIndex);
            row.WriteCells(0, values);
        }

        public void WriteRows(int rowIndex, List<object[]> arrayList)
        {
            int total = arrayList.Count;
            IRow row = this.CreateRows(rowIndex, total);
            for (int i = 0; i < total; i++)
            {
                row.WriteCells(0, arrayList[i]);
                row = this._sheet.GetRow(row.RowNum + 1);
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
                this.WriteRow(rowId, record);
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
            this._sheet.CopyRows(this._templateSheet, rowIndex, this._rowId, total);
            this._rowId += total;
            return this._sheet.GetRow(this._rowId - total);
        }

        #endregion Row related methods

        public void CreateFreezePane(int colSplit, int rowSplit)
        {
            this._sheet.CreateFreezePane(colSplit, rowSplit);
        }
    }
}