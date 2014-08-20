// --------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;

    /// <summary>
    /// Utilities class.
    /// </summary>
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
                var availableToDate = new DateTime(date.AddMonths(3).Year, date.AddMonths(3).Month, 1).AddDays(-1);
                return availableToDate;
            }
        }

        public static IList<SelectListItem> CheckInOutTypes
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem() { Text = Fanex.NexHR.Resources.Labels.CheckIn, Value = ((short)RequestType.CheckIn).ToString() },
                        new SelectListItem() { Text = Fanex.NexHR.Resources.Labels.CheckOut, Value = ((short)RequestType.CheckOut).ToString() }
                    };
            }
        }

        public static IList<SelectListItem> LateInEarlyOutTypes
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem() { Text = Fanex.NexHR.Resources.Labels.LateIn, Value = ((short)RequestType.LateIn).ToString() },
                        new SelectListItem() { Text = Fanex.NexHR.Resources.Labels.EarlyOut, Value = ((short)RequestType.EarlyOut).ToString() }
                    };
            }
        }

        public static DateTime AddTimeToDate(DateTime dateTime, string time)
        {
            var times = time.Split(new[] { ':' });
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

        /// <summary>
        /// Read text file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The text file content.</returns>
        public static string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Random string.
        /// </summary>
        /// <param name="length">The string length.</param>
        /// <returns>The return string.</returns>
        public static string RandomString(int length)
        {
            // returns random string of length specified with characters including: 0-9, a-z, A-Z
            var ca = new char[length];
            var random = new byte[length];

            // RNGCryptoServiceProvider is an implementation of a random number generator.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(random); // The array is now filled with cryptographically strong random bytes.

            for (var i = 0; i < length; i++)
            {
                var found = false;
                var rand = (int)random[i];
                while (!found)
                {
                    if (((rand >= 48) && (rand <= 57)) || ((rand >= 65) && (rand <= 90)) || ((rand >= 97) && (rand <= 122)))
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

            return new string(ca).ToLower();
        }

        /// <summary>
        /// Get minute steps.
        /// </summary>
        /// <param name="step">Step value.</param>
        /// <returns>List of select list item.</returns>
        public static IList<SelectListItem> MinuteSteps(int step)
        {
            var list = new List<SelectListItem>();
            var index = 60;
            while (index > 15)
            {
                list.Add(new SelectListItem()
                {
                    Text = index + "'",
                    Value = index.ToString()
                });
                index -= step;
            }

            return list;
        }

        /// <summary>
        /// The time steps.
        /// </summary>
        /// <param name="minuteStep">The minute step.</param>
        /// <returns>List of select list item.</returns>
        public static IList<SelectListItem> TimeSteps(int minuteStep)
        {
            var list = new List<SelectListItem>();
            var hour = 0;
            var minute = 0;

            while (hour < 24)
            {
                var value = ZeroPrefix(hour) + ":" + ZeroPrefix(minute);
                list.Add(new SelectListItem()
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

        /// <summary>
        /// The zero prefix.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The return string.</returns>
        public static string ZeroPrefix(int value)
        {
            return value < 10 ? "0" + value : value.ToString();
        }

        /// <summary>
        /// The zero prefix.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The return string.</returns>
        public static string ZeroPrefix(string value)
        {
            return int.Parse(value) < 10 ? "0" + value : value;
        }

        /// <summary>
        /// Convert array of string to string.
        /// </summary>
        /// <param name="array">Array of string.</param>
        /// <returns>The return string.</returns>
        public static string StringArrayToString(string[] array)
        {
            if (array == null || array.Length == 0)
            {
                return string.Empty;
            }

            var result = string.Empty;

            foreach (string item in array)
            {
                result += item + ",";
            }

            return result.Substring(0, result.Length - 1);
        }

        /// <summary>
        /// Is valid email or not.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>True : valid, false : invalid.</returns>
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
        /// Convert total hours to time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The return string.</returns>
        public static string TotalHoursToTime(float value)
        {
            int hour = (int)value;
            int minute = (int)Math.Round((value - hour) * 60);
            return hour > 0
                ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                : ZeroPrefix(minute) + "'";
        }

        /// <summary>
        /// Convert minute to time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The return string.</returns>
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
    }
}
