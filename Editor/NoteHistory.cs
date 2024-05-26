using System;

namespace GBG.ProjectNotes.Editor
{
    [Serializable]
    public class NoteHistory
    {
        public long timestamp;
        public string content;

        public NoteHistory(long timestamp, string content)
        {
            this.timestamp = timestamp;
            this.content = content;
        }
    }
}