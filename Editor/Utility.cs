using System;

namespace GBG.ProjectNotes.Editor
{
    public static class Utility
    {
        public static string NewGuid() => Guid.NewGuid().ToString();
        public static long NewTimestamp() => DateTime.UtcNow.Ticks;


        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

        public static int MinTitleLength = 4;
        public static int MinContentLength = 8;

        public static bool IsNiceTitle(string title, out string alert)
        {
            if (title.Length < MinTitleLength)
            {
                alert = $"Short title may lack recognizability.";
                return false;
            }

            alert = null;
            return true;
        }

        public static bool IsNiceContent(string content, out string alert)
        {
            if (content.Length < MinContentLength)
            {
                alert = $"Short content may be difficult to understand.";
                return false;
            }

            alert = null;
            return true;
        }


        public const string RedDotIconName = "redLight"; // "winbtn_mac_close"
    }
}