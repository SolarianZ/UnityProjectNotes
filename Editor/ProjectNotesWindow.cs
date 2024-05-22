using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class ProjectNotesWindow : EditorWindow, IHasCustomMenu
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


        private static Image _toolbarEntryRedDotIcon;
        private static bool _toolbarEntryRedDotIconVisible;
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

        private static void TryCreateToolbarEntry()
        {
            if (_toolbarEntryRedDotIcon != null)
            {
                return;
            }

            Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            object toolbarObj = toolbarType.GetField("get").GetValue(null);
            VisualElement toolbarRoot = (VisualElement)toolbarType.GetField("m_Root",
                BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(toolbarObj);
            VisualElement toolbarZoneLeft = toolbarRoot.Q("ToolbarZoneLeftAlign");
            VisualElement customToolbarLeft = toolbarZoneLeft.Q("custom-toolbar-left");
            VisualElement parent = customToolbarLeft ?? toolbarZoneLeft;

            EditorToolbarButton entryButton = new EditorToolbarButton(Open)
            {
                name = "ProjectNotesButton",
                icon = EditorGUIUtility.Load(EditorGUIUtility.isProSkin ? "d_console.infoicon.sml" : "console.infoicon.sml") as Texture2D,
                style =
                    {
                        marginLeft = 10,
                        marginRight = 10,
                    }
            };
            parent.Add(entryButton);

            Image entryIconImage = entryButton.Q<Image>();
            entryIconImage.style.alignItems = Align.FlexEnd;

            _toolbarEntryRedDotIconVisible = false;
            _toolbarEntryRedDotIcon = new Image
            {
                //image = EditorGUIUtility.Load("winbtn_mac_close") as Texture,
                image = EditorGUIUtility.Load("redLight") as Texture,
                style =
                {
                    marginRight = -2,
                    width = 9,
                    height = 9,
                    display = _toolbarEntryRedDotIconVisible ? DisplayStyle.Flex : DisplayStyle.None,
                }
            };
            entryIconImage.Add(_toolbarEntryRedDotIcon);
        }

        private static void UpdateToolbarEntryIcon(bool hasUnreadNotes)
        {
            if (hasUnreadNotes && !_toolbarEntryRedDotIconVisible)
            {
                _toolbarEntryRedDotIconVisible = true;
                _toolbarEntryRedDotIcon.style.display = DisplayStyle.Flex;
            }
            else if (!hasUnreadNotes && _toolbarEntryRedDotIconVisible)
            {
                _toolbarEntryRedDotIcon.style.display = DisplayStyle.None;
            }
        }

        private static void UpdateWindowTitleIcon(bool hasUnreadNotes)
        {
            if (!_windowInstance)
            {
                return;
            }

            // TODO: Update Window Title Icon
        }

        #endregion


        private bool _createSettingsButtonVisible;
        private Button _createSettingsButton;
        private VisualElement _mainViewContainer;
        private ProjectNoteContentView _contentView;


        private void OnEnable()
        {
            titleContent = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_console.infoicon.sml" : "console.infoicon.sml");
            titleContent.text = "Project Notes";
            minSize = new Vector2(200, 200);
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;


            #region Create New Settings

            _createSettingsButtonVisible = true;
            _createSettingsButton = new Button(ProjectNotesSettings.CreateNewInstance)
            {
                text = "Create New Settings",
                style =
                {
                    display = _createSettingsButtonVisible ? DisplayStyle.Flex : DisplayStyle.None,
                }
            };
            root.Add(_createSettingsButton);

            #endregion


            _mainViewContainer = new VisualElement
            {
                name = "MainViewContainer",
                style =
                {
                    display = _createSettingsButtonVisible ? DisplayStyle.None : DisplayStyle.Flex,
                }
            };
            root.Add(_mainViewContainer);
            _mainViewContainer.StretchToParentSize();


            #region Category

            VisualElement categoryContainer = new VisualElement
            {
                name = "CategoryContainer",
                style =
                {
                    flexDirection = FlexDirection.Row,
                },
            };
            _mainViewContainer.Add(categoryContainer);

            EditorToggleGroup categoryGroup = new EditorToggleGroup
            {
                name = "CategoryGroup",
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.Wrap,
                    flexGrow = 1,
                }
            };
            categoryContainer.Add(categoryGroup);

            categoryGroup.Add(new ToolbarToggle { text = "A111111111111" });
            categoryGroup.Add(new ToolbarToggle { text = "B111111111111" });
            categoryGroup.Add(new ToolbarToggle { text = "C111111111111" });
            categoryGroup.Add(new ToolbarToggle { text = "D111111111111" });
            categoryGroup.Add(new ToolbarToggle { text = "E111111111111" });
            categoryGroup.Add(new ToolbarToggle { text = "F111111111111" });

            #endregion


            TwoPaneSplitView noteContainer = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal)
            {
                name = "NoteContainer",
            };
            noteContainer.RegisterCallback<GeometryChangedEvent>(SetupNotePanes);
            _mainViewContainer.Add(noteContainer);


            #region Note List

            VisualElement temp1 = new VisualElement();
            noteContainer.Add(temp1);

            #endregion


            #region Note Content

            ProjectNoteContentView _contentView = new ProjectNoteContentView();
            noteContainer.Add(_contentView);

            #endregion
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

        private void ShowButton(Rect position)
        {
            if (GUI.Button(position, EditorGUIUtility.IconContent("_Help"), GUI.skin.FindStyle("IconButton")))
            {
                Application.OpenURL("https://github.com/SolarianZ/UnityProjectNotes");
            }
        }

        // Fix fixedPane null exception on Unity 2021
        private void SetupNotePanes(GeometryChangedEvent evt)
        {
            TwoPaneSplitView resultContainer = (TwoPaneSplitView)evt.target;
            resultContainer.UnregisterCallback<GeometryChangedEvent>(SetupNotePanes);

            // NullReferenceException thrown when no pane added
            resultContainer.schedule.Execute(() =>
            {
                resultContainer.fixedPane.style.minWidth = 200;
                resultContainer.flexedPane.style.minWidth = 200;
            });
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