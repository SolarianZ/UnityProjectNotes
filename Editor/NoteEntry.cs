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


        // Allow to clear remote read status
        public void UpdateGuid()
        {
            timestamp = Utility.NewTimestamp();
        }

        public void Update(NoteEntry newNote, bool addOldContentToHistory = true)
        {
            Assert.IsTrue(newNote.guid == guid);

            if (addOldContentToHistory && contentHistory.Count < MaxHistoryLength)
            {
                contentHistory.Add(new NoteHistory(timestamp, content));
            }

            timestamp = newNote.timestamp;
            category = newNote.category;
            author = newNote.author;
            isDraft = newNote.isDraft;
            priority = newNote.priority;
            title = newNote.title;
            content = newNote.content;
        }

        public NoteKey GetKey()
        {
            return new NoteKey(guid, timestamp);
        }
    }
}