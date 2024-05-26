using System;

namespace GBG.ProjectNotes.Editor
{
    [Serializable]
    public struct NoteKey : IEquatable<NoteKey>
    {
        public string guid;
        public long timestamp;

        public NoteKey(string guid, long timestamp)
        {
            this.guid = guid;
            this.timestamp = timestamp;
        }

        public override string ToString()
        {
            return $"{guid}&{timestamp}";
        }

        public string ToStringFormat(string format = null)
        {
            format ??= Utility.DateTimeFormat;
            return $"{guid}&{new DateTime(timestamp).ToString(format)}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(guid, timestamp);
        }

        public override bool Equals(object obj)
        {
            return obj is NoteKey key && Equals(key);
        }

        public bool Equals(NoteKey other)
        {
            return guid == other.guid && timestamp == other.timestamp;
        }

        public static bool operator ==(NoteKey left, NoteKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NoteKey left, NoteKey right)
        {
            return !left.Equals(right);
        }
    }
}