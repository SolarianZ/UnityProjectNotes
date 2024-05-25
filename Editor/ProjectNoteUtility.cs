using System;

namespace GBG.ProjectNotes.Editor
{
    public static class ProjectNoteUtility
    {
        public static long NewGuid() => DateTime.UtcNow.Ticks;
    }
}