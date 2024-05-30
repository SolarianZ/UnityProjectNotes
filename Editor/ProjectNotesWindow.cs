using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UDebug = UnityEngine.Debug;

namespace GBG.ProjectNotes.Editor
{
    public partial class ProjectNotesWindow : EditorWindow, IHasCustomMenu
    {
        #region Static

        public const string SearchPattern_Title = "title: ";
        public const string SearchPattern_Content = "content: ";
        public const string SearchPattern_Author = "author: ";
        public static ProjectNotesSettings Settings => ProjectNotesSettings.instance;
        internal static ProjectNotesLocalCache LocalCache => ProjectNotesLocalCache.instance;
        private static ProjectNotesWindow _windowInstance;


        [MenuItem("Tools/Bamboo/Project Notes")]
        public static void Open()
        {
            _windowInstance = GetWindow<ProjectNotesWindow>();
        }

        public static bool HasUnreadNotes()
        {
            if (!Settings)
            {
                return false;
            }

            bool hasUnreadNotes = LocalCache.HasUnreadNotes(Settings.Notes);
            return hasUnreadNotes;
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            TryCreateToolbarEntry();

            bool hasUnreadNotes = HasUnreadNotes();
            UpdateToolbarEntryIcon(hasUnreadNotes);
            UpdateWindowTitleIcon(hasUnreadNotes);
        }

        #endregion


        private readonly List<NoteEntry> _filteredNotes = new List<NoteEntry>();


        private void OnEnable()
        {
            titleContent = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_console.infoicon.sml" : "console.infoicon.sml");
            titleContent.text = "Project Notes";
            minSize = new Vector2(200, 200);

            LocalCache.RemoveInvalidGuids(Settings?.Notes);
            _filteredNotes.Clear();
        }

        private void OnFocus()
        {
            UpdateViews(_noteEntryListView?.selectedItem as NoteEntry);
        }

        private void Update()
        {
            if (!Settings && !_createSettingsButtonVisible)
            {
                _createSettingsButtonVisible = true;
                _createSettingsButton.style.display = DisplayStyle.Flex;
                _mainViewContainer.style.display = DisplayStyle.None;
            }
            else if (Settings && _createSettingsButtonVisible)
            {
                _createSettingsButtonVisible = false;
                _createSettingsButton.style.display = DisplayStyle.None;
                _mainViewContainer.style.display = DisplayStyle.Flex;
            }
        }


        private void OnSearchContentChanged(ChangeEvent<string> evt)
        {
            UDebug.LogError($"TODO : Search Notes '{evt.newValue}'");
        }

        private void SaveNote(NoteEntry noteToSave, bool isNewNote)
        {
            if (!Settings)
            {
                return;
            }

            if (isNewNote)
            {
                Settings.Notes.Add(noteToSave);
                Settings.ForceSave();
                LocalCache.MarkAsRead(noteToSave.GetKey());
                LocalCache.SelectedCategory = noteToSave.categoryTrimmed;
                UpdateViews(noteToSave);
                UDebug.Log($"[Project Notes] Note added: {noteToSave.title} {Utility.FormatTimestamp(noteToSave.timestamp)}.");
                return;
            }

            for (int i = 0; i < Settings.Notes.Count; i++)
            {
                NoteEntry note = Settings.Notes[i];
                if (note.guid != noteToSave.guid)
                {
                    continue;
                }

                note.Update(noteToSave);
                Settings.ForceSave();
                LocalCache.MarkAsRead(note.GetKey());
                UpdateViews(note);
                UDebug.Log($"[Project Notes] Note updated: {note.title} {Utility.FormatTimestamp(note.timestamp)}.");
                return;
            }

            UDebug.LogError($"[Project Notes] Failed to save note: {noteToSave.title} {Utility.FormatTimestamp(noteToSave.timestamp)}.");
        }

        private void DeleteNote(NoteEntry noteToDelete, long timestampToDelete)
        {
            if (!Settings)
            {
                return;
            }

            bool deleted = false;
            bool deletedHistory = false;
            for (int i = 0; i < Settings.Notes.Count; i++)
            {
                NoteEntry note = Settings.Notes[i];
                if (note != noteToDelete)
                {
                    continue;
                }

                if (note.timestamp == timestampToDelete)
                {
                    Settings.Notes.RemoveAt(i);
                    deleted = true;
                    break;
                }

                bool breakOuter = false;
                for (int j = 0; j < note.contentHistory.Count; j++)
                {
                    NoteHistory history = note.contentHistory[j];
                    if (history.timestamp == timestampToDelete)
                    {
                        note.contentHistory.RemoveAt(j);
                        deleted = true;
                        deletedHistory = true;
                        breakOuter = true;
                        break;
                    }
                }
                if (breakOuter)
                {
                    break;
                }
            }

            if (deleted)
            {
                Settings.ForceSave();

                if (deletedHistory)
                {
                    UDebug.Log($"[Project Notes] Note history deleted: {noteToDelete.title} {Utility.FormatTimestamp(timestampToDelete)}.");
                    _contentView.RefreshView();
                }
                else
                {
                    UDebug.Log($"[Project Notes] Note deleted: {noteToDelete.title} {Utility.FormatTimestamp(timestampToDelete)}.");
                    NoteEntry selection = null;
                    if (_filteredNotes.Count > 1)
                    {
                        NoteEntry candidate = _filteredNotes[0];
                        if (candidate == noteToDelete)
                        {
                            candidate = _filteredNotes[1];
                        }
                        selection = candidate;
                    }
                    UpdateViews(selection);
                }
            }
            else
            {
                UDebug.LogError($"[Project Notes] Failed to delete note: {noteToDelete.title} {Utility.FormatTimestamp(timestampToDelete)}.");
            }
        }


        #region Context Menu

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            // Document
            menu.AddItem(new GUIContent("Unity Manual: Supported rich text tags"), false, () =>
            {
                Application.OpenURL("https://docs.unity3d.com/Manual/UIE-supported-tags.html");
            });
            menu.AddSeparator("");

            // Debug
            menu.AddItem(new GUIContent("[Debug] Inspect Settings Asset"), false, () =>
            {
                Selection.activeObject = Settings;
            });
            menu.AddItem(new GUIContent("[Debug] Inspect Local Cache Asset"), false, () =>
            {
                Selection.activeObject = LocalCache;
            });
        }

        #endregion
    }
}