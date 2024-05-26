using System;
using System.Collections.Generic;

namespace GBG.ProjectNotes.Editor
{
    [Serializable]
    public class NoteEntry
    {
        public static int MaxHistoryLength = 8;

        public string guid = Utility.NewGuid();
        public long timestamp = Utility.NewTimestamp();
        public string title;
        public string content;
        public string category;
        public string author;
        public List<NoteHistory> contentHistory = new List<NoteHistory>();
        // TODO: public bool isDraft;


        // Allow to clear remote read status
        public void UpdateGuid()
        {
            timestamp = Utility.NewTimestamp();
        }

        public void UpdateContent(string newContent, bool addOldContentToHistory = true)
        {
            if (addOldContentToHistory && contentHistory.Count < MaxHistoryLength)
            {
                contentHistory.Add(new NoteHistory(timestamp, content));
            }

            timestamp = Utility.NewTimestamp();
            content = newContent;
        }

        public string GetDateTimeString()
        {
            return new DateTime(timestamp).ToString(Utility.DateTimeFormat);
        }

        public NoteKey GetKey()
        {
            return new NoteKey(guid, timestamp);
        }
    }
}