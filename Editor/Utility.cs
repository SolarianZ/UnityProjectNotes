using System;
using System.Collections.Generic;

namespace GBG.ProjectNotes.Editor
{
    public static class Utility
    {
        public static string NewGuid() => Guid.NewGuid().ToString();
        public static long NewTimestamp() => DateTime.UtcNow.Ticks;


        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";
        public static float ButtonBorderRadius = 2;

        public static string FormatTimestamp(long timestamp)
        {
            return new DateTime(timestamp).ToString(DateTimeFormat);
        }

        public static void CollectHistoryTimestamps(NoteEntry note, List<long> timestamps)
        {
            timestamps.Clear();
            if (note == null)
            {
                //timestamps.Add(0);
                return;
            }

            timestamps.Add(note.timestamp);
            foreach (NoteHistory history in note.contentHistory)
            {
                timestamps.Add(history.timestamp);
            }
        }

        public static bool HasUnreadNotesInCategory(string category)
        {
            ProjectNotesSettings settings = ProjectNotesSettings.instance;
            if (!settings)
            {
                return false;
            }

            category = category?.Trim();
            if (category == ProjectNotesSettings.CategoryAll)
            {
                foreach (NoteEntry note in settings.Notes)
                {
                    if (!ProjectNotesLocalCache.instance.IsRead(note.GetKey()))
                    {
                        return true;
                    }
                }
            }

            foreach (NoteEntry note in settings.Notes)
            {
                if (note.GetTrimmedCategory() == category &&
                    !ProjectNotesLocalCache.instance.IsRead(note.GetKey()))
                {
                    return true;
                }
            }

            return false;
        }


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