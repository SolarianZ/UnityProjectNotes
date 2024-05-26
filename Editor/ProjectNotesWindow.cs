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
            UpdateViews();
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

            // Document
            menu.AddItem(new GUIContent("Unity Manual: Supported rich text tags"), false, () =>
            {
                Application.OpenURL("https://docs.unity3d.com/Manual/UIE-supported-tags.html");
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