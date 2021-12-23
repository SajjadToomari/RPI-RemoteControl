using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RaspberryPi.Web.Utility
{
    public static class StaticMethods
    {
        public static string ToShamsi(this DateTime date, bool reverse = false)
        {
            var pc = new PersianCalendar();
            if (reverse)
            {
                return pc.GetHour(date) + ":" + (pc.GetMinute(date).ToString().Length == 1 ? "0" + pc.GetMinute(date) : pc.GetMinute(date).ToString()) + ":" + (pc.GetSecond(date).ToString().Length == 1 ? "0" + pc.GetSecond(date) : pc.GetSecond(date).ToString()) + " "
                    + pc.GetYear(date) + "/" + pc.GetMonth(date) + "/" + pc.GetDayOfMonth(date);
            }
            return pc.GetYear(date) + "/" + pc.GetMonth(date) + "/" + pc.GetDayOfMonth(date) + " "
                + pc.GetHour(date) + ":" + (pc.GetMinute(date).ToString().Length == 1 ? "0" + pc.GetMinute(date) : pc.GetMinute(date).ToString()) + ":" + (pc.GetSecond(date).ToString().Length == 1 ? "0" + pc.GetSecond(date) : pc.GetSecond(date).ToString());
        }
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
        public static string FixNameForSaveFile(this string text)
        {
            return text.Trim().Replace(" ", "_").Replace("/", "").Replace("\\", "");
        }

        public static string ToBase64(this byte[] binary)
        {
            return Convert.ToBase64String(binary);
        }

        public static byte[] FromBase64(this string str)
        {
            return Convert.FromBase64String(str);
        }
        static readonly object lockObject = new object();
        public static void WriteLog(string message, string fileName = "\\ServiceLog.txt", bool AddTime = true)
        {

            try
            {

                if (Monitor.TryEnter(lockObject, 300))
                {
                    try
                    {
                        var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + fileName, true);
                        if (AddTime)
                        {
                            sw.WriteLine(DateTime.Now + ": " + message);
                        }
                        else
                        {
                            sw.WriteLine(message);
                        }
                        sw.Flush();
                        sw.Close();
                    }
                    finally
                    {
                        Monitor.Exit(lockObject);
                    }
                }
                else
                {
                    // Code to execute if the attempt times out.  
                }

            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}