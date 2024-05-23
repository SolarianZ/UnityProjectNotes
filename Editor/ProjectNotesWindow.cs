using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public partial class ProjectNotesWindow : EditorWindow, IHasCustomMenu
    {
        #region Static

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


        private readonly List<ProjectNoteItem> _filteredNotes = new List<ProjectNoteItem>();


        private void OnEnable()
        {
            titleContent = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_console.infoicon.sml" : "console.infoicon.sml");
            titleContent.text = "Project Notes";
            minSize = new Vector2(200, 200);

            _filteredNotes.Clear();
            _filteredNotes.AddRange(Settings.Notes);
        }

        private void OnFocus()
        {
            // TODO: refresh view, need check null!
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


        #region Context Menu

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            // Settings
            menu.AddItem(new GUIContent("Inspect Settings Asset"), false, () =>
            {
                Selection.activeObject = Settings;
            });
            menu.AddSeparator("");

            // Debug
            menu.AddItem(new GUIContent("[Debug] Inspect Local Cache Asset"), false, () =>
            {
                Selection.activeObject = LocalCache;
            });
        }

        #endregion
    }
}