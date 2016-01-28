// ----------------------------------------------------------------------
// <copyright file="HSSFCellRangeAddressExtensions.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.27.01">Created.</change>
// </history>

namespace Fanex.NexHR.Core.NPOIExtensions
{
    using NPOI.SS.Util;

    public static class HSSFCellRangeAddressExtensions
    {
        public static CellRangeAddress Clone(this CellRangeAddressBase range)
        {
            var result = new CellRangeAddress(range.FirstRow, range.LastRow, range.FirstColumn, range.LastColumn);
            return result;
        }
    }
}