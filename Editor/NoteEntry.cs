using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.ProjectNotes.Editor
{
    [Serializable]
    public class NoteEntry
    {
        public static int MaxHistoryLength = 8;

        public string guid = Utility.NewGuid();
        public long timestamp = Utility.NewTimestamp();
        public string category;
        public string author;
        public bool isDraft;
        //[HideInInspector]
        //public string password;
        public string title;
        [TextArea(3, 20)]
        public string content;
        public List<NoteHistory> contentHistory = new List<NoteHistory>();


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

        public NoteKey GetKey()
        {
            return new NoteKey(guid, timestamp);
        }
    }
}