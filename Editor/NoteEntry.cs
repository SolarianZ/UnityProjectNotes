using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
        public int priority;
        public string title;
        [TextArea(3, 20)]
        public string content;
        public List<NoteHistory> contentHistory = new List<NoteHistory>();

        public string categoryTrimmed => category?.Trim();
        public long displayPriority { get; set; }


        // Allow to clear remote read status
        public void UpdateGuid()
        {
            timestamp = Utility.NewTimestamp();
        }

        public void Update(NoteEntry srcNote, bool addOldContentToHistory = true)
        {
            Assert.IsTrue(srcNote.guid == guid);

            if (addOldContentToHistory && contentHistory.Count < MaxHistoryLength)
            {
                contentHistory.Add(new NoteHistory(timestamp, content));
            }

            timestamp = srcNote.timestamp;
            category = srcNote.category;
            author = srcNote.author;
            isDraft = srcNote.isDraft;
            priority = srcNote.priority;
            title = srcNote.title;
            content = srcNote.content;
        }

        public void CopyFrom(NoteEntry srcNote)
        {
            guid = srcNote.guid;
            timestamp = srcNote.timestamp;
            category = srcNote.category;
            author = srcNote.author;
            isDraft = srcNote.isDraft;
            priority = srcNote.priority;
            title = srcNote.title;
            content = srcNote.content;
            contentHistory ??= new List<NoteHistory>();
            contentHistory.Clear();
            contentHistory.AddRange(srcNote.contentHistory);
        }

        public NoteKey GetKey()
        {
            return new NoteKey(guid, timestamp);
        }
    }
}