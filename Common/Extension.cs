using System;
using System.Text;

namespace Common
{
    public static class Extension
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public static string GetAllExceptionMessages(this Exception ex)
        {
            var msg = new StringBuilder();
            msg.Append(ex.Message);

            if (ex.InnerException != null)
            {
                msg.Append(", ");
                msg.Append(GetAllExceptionMessages(ex.InnerException));
            }

            return msg.ToString();
        }

        public static string ToFormattedString(this Exception ex)
        {
            return $"{nameof(Exception)}, {ex.GetAllExceptionMessages()}";
        }

        public static string ToFormattedString(this DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormat);
        }
    }
}