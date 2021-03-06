﻿using System;
using System.Text;

namespace BerryCore.Extensions
{
    /// <summary>
    /// 日期操作扩展方法
    /// </summary>
    public static class DateTimeExtension
    {
        private const int SECOND = 1;
        private const int MINUTE = 60 * SECOND;
        private const int HOUR = 60 * MINUTE;
        private const int DAY = 24 * HOUR;
        private const int MONTH = 30 * DAY;

        #region Json日期转换

        /// <summary>
        /// Json 的日期格式与.Net DateTime类型的转换
        /// </summary>
        /// <param name="jsonDate">Date(1242357713797+0800)</param>
        /// <returns></returns>
        public static DateTime JsonToDateTime(this string jsonDate)
        {
            string value = jsonDate.Substring(5, jsonDate.Length - 6) + "+0800";
            DateTimeKind kind = DateTimeKind.Utc;
            int index = value.IndexOf('+', 1);
            if (index == -1)
            {
                index = value.IndexOf('-', 1);
            }

            if (index != -1)
            {
                kind = DateTimeKind.Local;
                value = value.Substring(0, index);
            }
            long javaScriptTicks = long.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            long initialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            DateTime utcDateTime = new DateTime((javaScriptTicks * 10000) + initialJavaScriptDateTicks, DateTimeKind.Utc);
            DateTime dateTime;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                    break;

                case DateTimeKind.Local:
                    dateTime = utcDateTime.ToLocalTime();
                    break;

                default:
                    dateTime = utcDateTime;
                    break;
            }
            return dateTime;
        }

        #endregion Json日期转换

        #region 时间戳

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime TimeStamp2DateTime(this long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long DateTime2TimeStamp(this DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        #endregion

        /// <summary>
        /// 友好显示日期
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToFriendlyDateTime(this DateTime dateTime)
        {
            TimeSpan ts = DateTime.Now - dateTime;
            double delta = ts.TotalSeconds;
            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? "1秒前" : ts.Seconds + "秒前";
            }
            if (delta < 2 * MINUTE)
            {
                return "1分钟之前";
            }
            if (delta < 45 * MINUTE)
            {
                return ts.Minutes + "分钟前";
            }
            if (delta < 90 * MINUTE)
            {
                return "1小时前";
            }
            if (delta < 24 * HOUR)
            {
                return ts.Hours + "小时前";
            }
            if (delta < 48 * HOUR)
            {
                return "昨天";
            }
            if (delta < 30 * DAY)
            {
                return ts.Days + " 天之前";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "一个月之前" : months + "月之前";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "一年前" : years + "年前";
            }
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy-MM-dd HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="isRemoveSecond">是否移除秒</param>
        public static string ToDateTimeString(this DateTime dateTime, bool isRemoveSecond = false)
        {
            if (isRemoveSecond)
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm");
            }

            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy-MM-dd HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="isRemoveSecond">是否移除秒</param>
        public static string ToDateTimeString(this DateTime? dateTime, bool isRemoveSecond = false)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            return ToDateTimeString(dateTime.Value, isRemoveSecond);
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy-MM-dd"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy-MM-dd"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToDateString(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            return ToDateString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，不带年月日，格式："HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 获取格式化字符串，不带年月日，格式："HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToTimeString(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            return ToTimeString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，带毫秒，格式："yyyy-MM-dd HH:mm:ss.fff"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToMillisecondString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 获取格式化字符串，带毫秒，格式："yyyy-MM-dd HH:mm:ss.fff"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToMillisecondString(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            return ToMillisecondString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy年MM月dd日"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToChineseDateString(this DateTime dateTime)
        {
            return string.Format("{0}年{1}月{2}日", dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy年MM月dd日 HH时mm分"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="isRemoveSecond">是否移除秒</param>
        public static string ToChineseDateTimeString(this DateTime dateTime, bool isRemoveSecond = false)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}年{1}月{2}日", dateTime.Year, dateTime.Month, dateTime.Day);
            result.AppendFormat(" {0}时{1}分", dateTime.Hour, dateTime.Minute);
            if (isRemoveSecond == false)
            {
                result.AppendFormat("{0}秒", dateTime.Second);
            }

            return result.ToString();
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy年MM月dd日 HH时mm分"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="isRemoveSecond">是否移除秒</param>
        public static string ToChineseDateTimeString(this DateTime? dateTime, bool isRemoveSecond = false)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            return ToChineseDateTimeString(dateTime.Value);
        }

        /// <summary>
        /// 校验时间戳
        /// </summary>
        /// <param name="t">时间戳，10位</param>
        /// <returns></returns>
        public static bool CheckTimeStamp(this string t)
        {
            if (string.IsNullOrEmpty(t))
            {
                return false;
            }

            //DateTime time = DateTime.Now;
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //long now = (long)(time - startTime).TotalSeconds;
            //long arg = 0;
            //long.TryParse(t, out arg);

            //return Math.Abs(now - arg) <= 10;

            return true;
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime TimeStampToDateTime(this string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = Convert.ToInt64(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);

            DateTime time = dtStart.Add(toNow);

            return time;
        }

        /// <summary>
        /// 校验时间
        /// </summary>
        /// <param name="t">日期字符串</param>
        /// <returns></returns>
        public static bool CheckDateTime(this string t)
        {
            try
            {
                DateTime time;
                bool isSucc = DateTime.TryParse(t, out time);
                return isSucc;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 校验时间
        /// </summary>
        /// <param name="t">日期</param>
        /// <returns></returns>
        public static bool CheckDateTime(this DateTime t)
        {
            bool isSucc = t.Ticks > 0 && t.Year >= 1970;
            return isSucc;
        }
    }
}