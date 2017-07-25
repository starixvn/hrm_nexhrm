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
    using Enums;
    using Model;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    public static class Utilities
    {
        public static DateTime RequestAvailableFromDate
        {
            get
            {
                var date = DateTime.Now;
                var availableFromDate = new DateTime(date.AddMonths(-3).Year, date.AddMonths(-3).Month, 1);
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

        public static IList<SelectListItem> CheckInOutTypes => new List<SelectListItem>
        {
            new SelectListItem { Text = Labels.CheckIn, Value = ((short)RequestType.CheckIn).ToString() },
            new SelectListItem { Text = Labels.CheckOut, Value = ((short)RequestType.CheckOut).ToString() }
        };

        public static IList<SelectListItem> LateInEarlyOutTypes => new List<SelectListItem>
        {
            new SelectListItem { Text = Labels.LateIn, Value = ((short)RequestType.LateIn).ToString() },
            new SelectListItem { Text = Labels.EarlyOut, Value = ((short)RequestType.EarlyOut).ToString() }
        };

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
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static string RandomString(int length)
        {
            // returns random string of length specified with characters including: 0-9, a-z, A-Z
            var ca = new char[length];
            var random = new byte[length];

            // RNGCryptoServiceProvider is an implementation of a random number generator.
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(random); // The array is now filled with cryptographically strong random bytes.

                for (var i = 0; i < length; i++)
                {
                    var found = false;
                    var rand = (int)random[i];
                    while (!found)
                    {
                        if (rand.IsWithin(48,57) || rand.IsWithin(65,90) || rand.IsWithin(97,122))
                        {
                            found = true;
                        }
                        else
                        {
                            // get a new random int.
                            var single = new byte[1];
                            rng.GetBytes(single);
                            rand = single[0];
                        }
                    }

                    var ci = (char)rand;
                    ca[i] = ci;
                }

                return new string(ca).ToLower(CultureInfo.InvariantCulture);
            }
        }

        public static bool IsWithin(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }

        public static IList<SelectListItem> MinuteSteps(int step)
        {
            var list = new List<SelectListItem>();
            var index = 60;
            while (index > 15)
            {
                list.Add(new SelectListItem
                {
                    Text = string.Format(Labels.MinuteFormat, index),
                    Value = index.ToString()
                });
                index -= step;
            }

            return list;
        }

        public static IList<SelectListItem> TimeSteps(int minuteStep)
        {
            var list = new List<SelectListItem>();
            var hour = 6;
            var minute = 0;

            while (hour < 24)
            {
                var value = string.Format(Labels.HourFormat, ZeroPrefix(hour), ZeroPrefix(minute));
                list.Add(new SelectListItem
                {
                    Text = value,
                    Value = value
                });
                minute += minuteStep;
                if (minute >= 60)
                {
                    minute = 0;
                    hour++;
                }
            }

            return list;
        }

        public static string ZeroPrefix(object value)
        {
            return value.ToString().PadLeft(2, '0');
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
            IOrderedEnumerable<T> sorted = null;
            if (sortExpressions[index].Direction == "asc")
            {
                if (orderedQuery != null)
                {
                    sorted = (index == 0)
                        ? query.OrderBy(expression)
                        : orderedQuery.ThenBy(expression);
                }
            }
            else
            {
                sorted = (index == 0)
                    ? query.OrderByDescending(expression)
                    : orderedQuery?.ThenByDescending(expression);
            }

            return sorted;
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