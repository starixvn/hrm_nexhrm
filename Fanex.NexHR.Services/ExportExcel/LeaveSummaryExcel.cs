using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.NexHR.Models.AnnualLeave;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Fanex.NexHR.Services.ExportExcel;

public class LeaveSummaryExcel(string templatePath, List<LeaveSummary> leaveSummaries, int year, bool isHrRole): ExcelBase(templatePath, 0, "LeaveSummary")
{
    // Row constants
    private const byte RowStartHeader = 2;
    private const byte RowStartMonthHeader = 3;
    private const byte RowStartDepartmentHeader = 4;
    // Column constants
    private const byte ColEnglishName = 1;
    private const byte ColTeam = 2;
    private const byte ColPayrollCompany = 3;
    private byte ColYear => isHrRole ? (byte)4 : (byte)3;
    private byte ColTakeLeaveStart => isHrRole ? (byte)5 : (byte)4;
    private byte ColTakeLeaveEnd => isHrRole ? (byte)16 : (byte)15;
    private const byte TotalColsWithPayroll = 17;
    private const byte TotalColsWithoutPayroll = 16;
    private const byte TitleFontSize = 16;

    // Month columns - will be adjusted based on HR role
    private byte[] MonthColumns => isHrRole 
        ? new byte[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }  // With Payroll Company
        : new byte[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }; // Without Payroll Company
    private static readonly string[] MonthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    
    private int rowIndex;
    
    // Dynamic properties based on HR role
    private byte TotalCols => isHrRole ? TotalColsWithPayroll : TotalColsWithoutPayroll;
    private byte ColBalance => isHrRole ? (byte)17 : (byte)16;

    protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
    {
        worksheet.Cells[1, 1].Value = $"Annual Leave Summary - {year}";
        worksheet.Cells[1, 1].Style.Font.Size = TitleFontSize;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells[1, 1, 1, TotalCols].Merge = true;
    }

    protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
    {
        RenderMainHeaders(worksheet);
        RenderMonthHeaders(worksheet);
        ApplyHeaderBorders(worksheet);
    }

    private void RenderMainHeaders(ExcelWorksheet worksheet)
    {
        worksheet.Cells[RowStartHeader, ColEnglishName].Value = "English Name";
        worksheet.Cells[RowStartHeader, ColTeam].Value = "Team";
        
        if (isHrRole)
        {
            worksheet.Cells[RowStartHeader, ColPayrollCompany].Value = "Payroll Company";
        }
        
        worksheet.Cells[RowStartHeader, ColYear].Value = year.ToString();
        worksheet.Cells[RowStartHeader, ColTakeLeaveStart].Value = "Take Annual Leave " + year;
        worksheet.Cells[RowStartHeader, ColBalance].Value = "Balance of AL " + year;

        MergeHorizontally(worksheet, RowStartHeader, ColTakeLeaveStart, ColTakeLeaveEnd);
        MergeVertically(worksheet, RowStartHeader, RowStartMonthHeader, ColEnglishName);
        MergeVertically(worksheet, RowStartHeader, RowStartMonthHeader, ColTeam);
        
        if (isHrRole)
        {
            MergeVertically(worksheet, RowStartHeader, RowStartMonthHeader, ColPayrollCompany);
        }
        
        MergeVertically(worksheet, RowStartHeader, RowStartMonthHeader, ColYear);
        MergeVertically(worksheet, RowStartHeader, RowStartMonthHeader, ColBalance, wrapText: true);
    }

    private void RenderMonthHeaders(ExcelWorksheet worksheet)
    {
        for (var i = 0; i < MonthNames.Length; i++)
        {
            worksheet.Cells[RowStartMonthHeader, MonthColumns[i]].Value = MonthNames[i];
        }
    }

    private static void MergeHorizontally(ExcelWorksheet worksheet, int row, int startCol, int endCol, ExcelHorizontalAlignment alignment = ExcelHorizontalAlignment.Center)
    {
        var range = worksheet.Cells[row, startCol, row, endCol];
        range.Merge = true;
        range.Style.HorizontalAlignment = alignment;
    }

    private static void MergeVertically(ExcelWorksheet worksheet, int startRow, int endRow, int column, bool wrapText = false)
    {
        var cell = worksheet.Cells[startRow, column, endRow, column];
        cell.Merge = true;
        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        if (wrapText)
        {
            cell.Style.WrapText = true;
        }
    }

    private void ApplyHeaderBorders(ExcelWorksheet worksheet)
    {
        ApplyBorder(worksheet, RowStartHeader, 1, TotalCols);
        ApplyBorder(worksheet, RowStartMonthHeader, 1, TotalCols);
    }

    private static void ApplyBorder(ExcelWorksheet worksheet, int row, int startCol, int endCol)
    {
        var range = worksheet.Cells[row, startCol, row, endCol];
        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
    }

    protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
    {
        if (leaveSummaries?.Any() != true)
        {
            return;
        }

        rowIndex = RowStartDepartmentHeader;
        var leaveSummaryByDepartments = leaveSummaries.GroupBy(t => t.DepartmentId).Select(grp => grp.First());
        
        foreach (var leaveSummaryByDepartment in leaveSummaryByDepartments)
        {
            RenderDepartmentHeader(worksheet, leaveSummaryByDepartment.DepartmentName);
            RenderDepartmentEmployees(worksheet, leaveSummaryByDepartment.DepartmentId);
        }
    }

    private void RenderDepartmentHeader(ExcelWorksheet worksheet, string departmentName)
    {
        worksheet.Cells[rowIndex, ColEnglishName].Value = departmentName.ToUpperInvariant();

        MergeHorizontally(worksheet, rowIndex, ColEnglishName, TotalCols, ExcelHorizontalAlignment.Left);

        // Style department header
        worksheet.Cells[rowIndex, ColEnglishName].Style.Font.Bold = true;
        worksheet.Cells[rowIndex, ColEnglishName].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[rowIndex, ColEnglishName].Style.Fill.BackgroundColor.SetColor(Color.Beige);
        
        // Apply border to department header
        ApplyBorder(worksheet, rowIndex, 1, TotalCols);

        rowIndex++;
    }

    private void RenderDepartmentEmployees(ExcelWorksheet worksheet, int departmentId)
    {
        var leaveSummariesByEmp = leaveSummaries.Where(t => t.DepartmentId == departmentId);
        foreach (var leaveSummary in leaveSummariesByEmp)
        {
            RenderEmployeeRow(leaveSummary, worksheet);
            rowIndex++;
        }
    }

    protected internal virtual void RenderEmployeeRow(LeaveSummary leaveSummary, ExcelWorksheet worksheet)
    {
        // Basic info
        worksheet.Cells[rowIndex, ColEnglishName].Value = leaveSummary.NickName;
        worksheet.Cells[rowIndex, ColTeam].Value = leaveSummary.TeamCode;
        
        if (isHrRole)
        {
            worksheet.Cells[rowIndex, ColPayrollCompany].Value = leaveSummary.PayrollCompanyName ?? "";
        }
        
        worksheet.Cells[rowIndex, ColYear].Value = leaveSummary.AnnualLeaveCurrentYear.ToString("0.0");
        
        // Monthly data
        var monthlyValues = GetMonthlyValues(leaveSummary);
        for (var i = 0; i < monthlyValues.Length; i++)
        {
            worksheet.Cells[rowIndex, MonthColumns[i]].Value = monthlyValues[i];
        }
        
        // Balance
        worksheet.Cells[rowIndex, ColBalance].Value = FormatNumber(leaveSummary.LeaveBalance);
        
        // Apply border to employee row
        ApplyBorder(worksheet, rowIndex, 1, TotalCols);
    }

    private static string[] GetMonthlyValues(LeaveSummary leaveSummary)
    {
        return
        [
            FormatNumber(leaveSummary.Jan),
            FormatNumber(leaveSummary.Feb),
            FormatNumber(leaveSummary.Mar),
            FormatNumber(leaveSummary.Apr),
            FormatNumber(leaveSummary.May),
            FormatNumber(leaveSummary.Jun),
            FormatNumber(leaveSummary.Jul),
            FormatNumber(leaveSummary.Aug),
            FormatNumber(leaveSummary.Sep),
            FormatNumber(leaveSummary.Oct),
            FormatNumber(leaveSummary.Nov),
            FormatNumber(leaveSummary.Dec)
        ];
    }

    private static string FormatNumber(float value)
    {
        const float epsilon = 0.0001f;
        return Math.Abs(value) > epsilon ? value.ToString("0.0") : "-";
    }

    protected internal override void ProcessUI(ExcelWorksheet worksheet)
    {
        SetStyle(worksheet);
        SetWidthColumn(worksheet);
    }

    protected internal virtual void SetStyle(ExcelWorksheet worksheet)
    {
        StyleHeaderRows(worksheet);
        AlignNumericColumns(worksheet);
    }

    private void StyleHeaderRows(ExcelWorksheet worksheet)
    {
        StyleHeaderRow(worksheet, RowStartHeader);
        StyleHeaderRow(worksheet, RowStartMonthHeader);
    }

    private void StyleHeaderRow(ExcelWorksheet worksheet, int row)
    {
        var range = worksheet.Cells[row, 1, row, TotalCols];
        range.Style.Font.Bold = true;
        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(HeaderTitleColor);
        range.Style.Font.Color.SetColor(Color.White);
        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    }


    private void AlignNumericColumns(ExcelWorksheet worksheet)
    {
        // Center align year and monthly columns
        for (int col = ColYear; col <= ColTakeLeaveEnd; col++)
        {
            worksheet.Column(col).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        worksheet.Column(ColBalance).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        
        // Center align Payroll Company column if HR role
        if (isHrRole)
        {
            worksheet.Column(ColPayrollCompany).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }

    protected internal virtual void SetWidthColumn(ExcelWorksheet worksheet)
    {
        SetRowHeights(worksheet);
        SetColumnWidths(worksheet);
    }

    private static void SetRowHeights(ExcelWorksheet worksheet)
    {
        const int RowHeightTitle = 25;
        const int RowHeightHeader = 20;
        
        worksheet.Row(1).Height = RowHeightTitle;
        worksheet.Row(RowStartHeader).Height = RowHeightHeader;
        worksheet.Row(RowStartMonthHeader).Height = RowHeightHeader;
    }

    private void SetColumnWidths(ExcelWorksheet worksheet)
    {
        int[] widthCol = isHrRole 
            ? [20, 15, 20, 10, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 25]  // With Payroll Company
            : [20, 15, 10, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 25];     // Without Payroll Company
            
        for (var i = 0; i < widthCol.Length && i < TotalCols; i++)
        {
            worksheet.Column(i + 1).Width = widthCol[i];
        }
    }
}