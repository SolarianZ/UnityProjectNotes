using System;
using System.Collections.Generic;

namespace GBG.ProjectNotes.Editor
{
    [Serializable]
    public class ProjectNoteItem
    {
        public static int MaxHistoryLength = 8;
        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

        public long guid = DateTime.UtcNow.Ticks;
        public string title;
        public string content;
        public string category;
        public string author;
        public List<History> contentHistory = new List<History>();


        [Serializable]
        public class History
        {
            public long guid;
            public string content;

            public History(long guid, string content)
            {
                this.guid = guid;
                this.content = content;
            }
        }


        // Allow to clear remote read status
        public void UpdateGuid()
        {
            guid = DateTime.UtcNow.Ticks;
        }

        public void UpdateContent(string newContent, bool addOldContentToHistory = true)
        {
            if (addOldContentToHistory && contentHistory.Count < MaxHistoryLength)
            {
                contentHistory.Add(new History(guid, content));
            }

            guid = DateTime.UtcNow.Ticks;
            content = newContent;
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
    }
}