namespace Fanex.NexHR.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Enums;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Resources;

    public static class Extensions
    {
        public static string EnumToString(this Command value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs != null && attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        public static string EnumToString(this Role value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs != null && attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        public static string EnumToString(this RequestType value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs != null && attribs.Length > 0 ? attribs[0].ToString() : null;
        }

        public static string GetDisplayName(this Enum enumeration)
        {
            var type = enumeration.GetType();
            var memberInfo = type.GetMember(enumeration.ToString());

            // if there is member information
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

                if (attributes.Length > 0)
                {
                    return ((DisplayAttribute)attributes[0]).GetName();
                }
            }

            return enumeration.ToString();
        }

        public static MvcForm BeginGetForm(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.BeginForm(null, null, FormMethod.Get);
        }

        public static DateTime GetMaxDate(this DateTime date)
        {
            if (date.AddMonths(1).Year > DateTimeOffset.Now.Year)
            {
                return DateTimeOffset.Now.DateTime.AddYears(1);
            }

            return date;
        }

        public static List<SelectListItem> RequestTypes => new List<SelectListItem>
        {
            new SelectListItem { Text = Labels.All, Value = ((int)RequestTypeFilterForRequest.All).ToString() },
            new SelectListItem { Text = Labels.Leave, Value = ((int)RequestTypeFilterForRequest.Leave).ToString() },
            new SelectListItem { Text = Labels.CheckInOut, Value = ((int)RequestTypeFilterForRequest.CheckInOut).ToString() }
        };

        public static List<SelectListItem> YearPicker(
            int fromYear,
            int currentYear,
            bool hasFirstItem = false)
        {
            var listSelectYear = new List<SelectListItem>();
            if (hasFirstItem)
            {
                listSelectYear.Add(new SelectListItem
                {
                    Text = Labels.All,
                    Value = "-1",
                    Selected = true
                });
            }

            var toYear = DateTimeOffset.Now.DateTime.GetMaxDate().Year + 1;

            for (var index = toYear; index >= fromYear; index--)
            {
                listSelectYear.Add(new SelectListItem
                {
                    Text = index.ToString(),
                    Value = index.ToString(),
                    Selected = !hasFirstItem && (index == currentYear)
                });
            }

            return listSelectYear;
        }

        #region Business

        public static string ToText(this byte status)
        {
            string statusName;
            switch (status)
            {
                case (byte)RequestStatuse.Pending:
                    statusName = Labels.Pending;
                    break;

                case (byte)RequestStatuse.Approve:
                    statusName = Labels.Approved;
                    break;

                case (byte)RequestStatuse.Reject:
                    statusName = Labels.Rejected;
                    break;

                default:
                    statusName = string.Empty;
                    break;
            }

            return statusName;
        }

        public static string ToText(this short type)
        {
            string requestTypeName;
            switch (type)
            {
                case (short)RequestType.CheckIn:
                    requestTypeName = Labels.CheckIn;
                    break;

                case (short)RequestType.CheckOut:
                    requestTypeName = Labels.CheckOut;
                    break;

                case (short)RequestType.Leave:
                    requestTypeName = Labels.Leave;
                    break;

                default:
                    requestTypeName = string.Empty;
                    break;
            }

            return requestTypeName;
        }

        public static string ToFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToFormatedDate();
            }

            return string.Empty;
        }

        public static string ToFormatedDate(this DateTime date)
        {
            return date.ToString("dd MMM, yyyy hh:mm tt");
        }

        public static string ToShortFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToShortFormatedDate();
            }

            return string.Empty;
        }

        public static string ToFormattedDateTime(this DateTime date)
        {
            return date.ToString("dd MMM, yyyy HH:mm");
        }

        public static string ToFormattedDateTime(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString("dd MMM, yyyy HH:mm");
            }
            return string.Empty;
        }

        public static string ToShortFormatedDate(this DateTime date)
        {
            string dateStr = date.ToString(@"dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");
            return dateStr;
        }

        public static string ToDetailFormatedDate(this DateTime date)
        {
            string dateStr = date.ToString(@"ddd, dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");
            return dateStr;
        }

        public static string ToMonthAndDayFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndDayFormatedDate();
            }

            return string.Empty;
        }

        public static string ToMonthAndDayFormatedDate(this DateTime date)
        {
            return date.ToString("dd-MMM");
        }

        public static string ToMonthAndYearFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndYearFormatedDate();
            }

            return string.Empty;
        }

        public static string ToMonthAndYearFormatedDate(this DateTime date)
        {
            return date.ToString("MMM-yyyy");
        }

        public static string ToDateMonthYearFormatedDate(this DateTime date)
        {
            return date.ToString("dd MMM, yyyy");
        }

        public static string ToDateMonthYearFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToDateMonthYearFormatedDate();
            }

            return string.Empty;
        }

        #endregion Business
    }
}