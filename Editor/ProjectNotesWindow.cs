using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public partial class ProjectNotesWindow : EditorWindow, IHasCustomMenu
    {
        #region Static

        [MenuItem("Tools/Bamboo/Project Notes")]
        public static void Open()
        {
            _windowInstance = GetWindow<ProjectNotesWindow>();
        }

        public static bool HasUnreadNotes()
        {
            ProjectNotesSettings settings = ProjectNotesSettings.instance;
            if (!settings)
            {
                return false;
            }

            ProjectNotesLocalCache localCache = ProjectNotesLocalCache.instance;
            bool hasUnreadNotes = localCache.HasUnreadNotes(settings.Notes);
            return hasUnreadNotes;
        }


        private static ProjectNotesWindow _windowInstance;


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



        private void OnEnable()
        {
            titleContent = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_console.infoicon.sml" : "console.infoicon.sml");
            titleContent.text = "Project Notes";
            minSize = new Vector2(200, 200);
        }

        private void Update()
        {
            if (!ProjectNotesSettings.instance && !_createSettingsButtonVisible)
            {
                _createSettingsButtonVisible = true;
                _createSettingsButton.style.display = DisplayStyle.Flex;
                _mainViewContainer.style.display = DisplayStyle.None;
            }
            else if (ProjectNotesSettings.instance && _createSettingsButtonVisible)
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
                Selection.activeObject = ProjectNotesSettings.instance;
            });
            menu.AddSeparator("");

            // Debug
            menu.AddItem(new GUIContent("[Debug] Inspect Local Cache Asset"), false, () =>
            {
                Selection.activeObject = ProjectNotesLocalCache.instance;
            });
        }

        #endregion
    }
}