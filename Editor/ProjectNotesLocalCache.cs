using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GBG.ProjectNotes.Editor
{
    [FilePath("Library/com.greenbamboogames.projectnotes/LocalCache.asset",
        FilePathAttribute.Location.ProjectFolder)]
    internal class ProjectNotesLocalCache : ScriptableSingleton<ProjectNotesLocalCache>
    {
        #region Category

        [SerializeField]
        private string _selectedCategory;

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    ForceSave();
                }
            }
        }

        #endregion


        #region Note Status

        [SerializeField]
        private List<NoteKey> _readNoteKeys = new List<NoteKey>();


        public bool HasUnreadNotes(IEnumerable<NoteEntry> notes)
        {
            foreach (NoteEntry note in notes)
            {
                if (!IsRead(note.GetKey()))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsRead(NoteKey key)
        {
            return _readNoteKeys.Contains(key);
        }

        public void MarkAsRead(NoteKey key)
        {
            if (!_readNoteKeys.Contains(key))
            {
                _readNoteKeys.Add(key);
                Save(true);
            }
        }

        public void MarkAsUnread(NoteKey key)
        {
            if (_readNoteKeys.Remove(key))
            {
                Save(true);
            }
        }

        public void ClearReadNotes()
        {
            _readNoteKeys.Clear();
            Save(true);
        }

        public void RemoveInvalidGuids(IEnumerable<NoteEntry> notes)
        {
            for (int i = _readNoteKeys.Count - 1; i >= 0; i--)
            {
                bool invalid = true;
                foreach (NoteEntry note in notes)
                {
                    if (note.GetKey() == _readNoteKeys[i])
                    {
                        invalid = false;
                        break;
                    }
                }

                if (invalid)
                {
                    _readNoteKeys.RemoveAt(i);
                }
            }

            Save(true);
        }

        #endregion


        [ContextMenu("Force Save")]
        public void ForceSave()
        {
            Save(true);
        }
    }
}