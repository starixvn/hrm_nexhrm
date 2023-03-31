// --------------------------------------------------------------------------
// <copyright file="Utilities.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core
{
    using Fanex.NexHR.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class Utilities
    {
        public static DateTime RequestAvailableFromDate
        {
            get
            {
                var date = DateTime.Now;
                var availableFromDate = new DateTime(date.AddMonths(-12).Year, date.AddMonths(-12).Month, 1);
                return availableFromDate;
            }
        }

        public static DateTime RequestAvailableToDate
        {
            get
            {
                var date = DateTime.Now;
                var availableToDate = new DateTime(date.AddMonths(4).Year, date.AddMonths(4).Month, 1).AddDays(-1);
                return availableToDate;
            }
        }

        public static DateTime AddTimeToDate(DateTime dateTime, string time)
        {
            var times = time.Split(':');
            TimeSpan ts = new TimeSpan(int.Parse(times[0]), int.Parse(times[1]), 0);
            return dateTime.Date + ts;
        }

        public static string GetTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            var value = (DateTime)dateTime;
            return ZeroPrefix(value.Hour) + ":" + ZeroPrefix(value.Minute);
        }

        public static string ReadTextFile(string filePath)
        {
            if (filePath == null)
            {
                return string.Empty;
            }
            else
            {
                using (var sr = new StreamReader(filePath))
                {

                    return sr.ReadToEnd();
                }
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool IsWithin(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }

        public static string ZeroPrefix(object value)
        {
            const int totalWidth = 2;
            return value.ToString().PadLeft(totalWidth, '0');
        }

        public static string StringArrayToString(string[] array)
        {
            if (array == null || array.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            foreach (string item in array)
            {
                result.Append(item);
                result.Append(",");
            }

            return result.ToString(0, result.Length - 1);
        }

        public static IList<SelectListItem> TimeSteps(int minuteStep)
        {
            var list = new List<SelectListItem>();
            var hour = 6;
            var minute = 0;
            const int hours = 24;
            const int minutes = 60;

            while (hour < hours)
            {
                var value = string.Format(Labels.HourFormat, ZeroPrefix(hour), ZeroPrefix(minute));
                list.Add(new SelectListItem
                {
                    Text = value,
                    Value = value
                });
                minute += minuteStep;
                if (minute >= minutes)
                {
                    minute = 0;
                    hour++;
                }
            }

            return list;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }

        /// <summary>
        /// Display decimal numberic with time format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecimalToTime(decimal value)
        {
            decimal hours = Math.Truncate(value);
            decimal minutes = (value - hours) * 60;
            return ZeroPrefix((int)hours) + ":" + ZeroPrefix((int)minutes);
        }

        public static string TotalHoursToTime(float value)
        {
            int hour = (int)value;
            int minute = (int)Math.Round((value - hour) * 60);
            return hour > 0
                ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                : ZeroPrefix(minute) + "'";
        }

        public static string MinuteToTime(int value)
        {
            if (value == 0)
            {
                return string.Empty;
            }

            int hour = value / 60;
            int minute = value - (hour * 60);

            return hour > 0
                ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                : ZeroPrefix(minute) + "'";
        }

        public static IList<T> MultipleSort<T>(IList<T> data, IList<SortModel> sortExpressions)
        {
            // No sorting needed.
            if (sortExpressions == null || !sortExpressions.Any())
            {
                return data;
            }

            // Let us sort it.
            IEnumerable<T> query = data.Select(item => item);
            IOrderedEnumerable<T> orderedQuery = null;

            for (int i = 0; i < sortExpressions.Count; i++)
            {
                // We need to keep the loop index, not sure why it is altered by the Linq.
                var index = i;
                Func<T, object> expression = GetSortExpression<T>(sortExpressions, index);

                // Need to review the functionality of 'orderedQuery'
                orderedQuery = SortOrderedQuery(sortExpressions, query, orderedQuery, index, expression);
            }

            return orderedQuery?.ToList();
        }

        private static IOrderedEnumerable<T> SortOrderedQuery<T>(
            IList<SortModel> sortExpressions,
            IEnumerable<T> query,
            IOrderedEnumerable<T> orderedQuery,
            int index,
            Func<T, object> expression)
        {
            if (sortExpressions[index].Direction == "asc")
            {
                if (index == 0)
                {
                    return query.OrderBy(expression);
                }

                return orderedQuery?.ThenBy(expression);
            }
            else
            {
                if (index == 0)
                {
                    return query.OrderByDescending(expression);
                }

                return orderedQuery?.ThenByDescending(expression);
            }
        }

        private static Func<T, object> GetSortExpression<T>(IList<SortModel> sortExpressions, int index)
        {
            return item => item.GetType().GetProperty(sortExpressions[index].Property)?.GetValue(item, null);
        }

        public static string GetDescription(Enum enumType)
        {
            FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : enumType.ToString();
        }
    }
}