using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GBG.ProjectNotes.Editor
{
    [FilePath("Library/com.greenbamboogames.projectnotes/LocalCache.asset",
        FilePathAttribute.Location.ProjectFolder)]
    internal class ProjectNotesLocalCache : ScriptableSingleton<ProjectNotesLocalCache>
    {
        [SerializeField]
        private List<long> _readNoteGuids = new List<long>();


        public bool HasUnreadNotes(IEnumerable<ProjectNoteItem> notes)
        {
            foreach (ProjectNoteItem note in notes)
            {
                if (!IsRead(note.guid))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsRead(long guid)
        {
            return _readNoteGuids.Contains(guid);
        }

        public void MarkAsRead(long guid)
        {
            if (!_readNoteGuids.Contains(guid))
            {
                _readNoteGuids.Add(guid);
                Save(true);
            }
        }

        public void MarkAsUnread(long guid)
        {
            if (_readNoteGuids.Remove(guid))
            {
                Save(true);
            }
        }

        public void ClearReadNotes()
        {
            _readNoteGuids.Clear();
            Save(true);
        }

        public void RemoveInvalidGuids(IEnumerable<ProjectNoteItem> notes)
        {
            for (int i = _readNoteGuids.Count - 1; i >= 0; i--)
            {
                bool invalid = true;
                foreach (ProjectNoteItem note in notes)
                {
                    if (note.guid == _readNoteGuids[i])
                    {
                        invalid = false;
                        break;
                    }
                }

                if (invalid)
                {
                    _readNoteGuids.RemoveAt(i);
                }
            }

            Save(true);
        }

        [ContextMenu("Force Save")]
        public void ForceSave()
        {
            Save(true);
        }
    }
}