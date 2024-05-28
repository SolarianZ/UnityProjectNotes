using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UDebug = UnityEngine.Debug;

namespace GBG.ProjectNotes.Editor
{
    public class NoteContentView : VisualElement
    {
        public static Color draftLabelBorderColor = new Color32(255, 180, 0, 255);
        public static Color unreadLabelBorderColor = new Color32(240, 0, 0, 255);
        public static float statusLabelBorderWidth = 2;
        public static float statusLabelBorderRadius = 4;
        public static float contentLabelBorderSize = 1;
        public static Color contentLabelBorderColorDark = new Color32(44, 44, 44, 255);
        public static Color contentLabelBorderColorLight = new Color32(177, 177, 177, 255);

        private readonly Label _versionLabel;
        private readonly Label _unreadLabel;
        private readonly Label _titleLabel;
        private readonly Label _authorLabel;
        private readonly PopupField<long> _historyPopup;
        private readonly List<long> _historyTimestamps = new List<long>();
        private readonly Label _contentLabel;
        private readonly Button _markButton;
        private readonly Button _opDropdownButton;
        private NoteEntry _note;

        public event Action<NoteEntry> readStatusChanged;
        public event Action<NoteEntry> wantsToEditNote;
        public event Action<NoteEntry, long> wantsToDeleteNote;


        public NoteContentView()
        {
            style.paddingLeft = 8;
            style.paddingRight = 6;
            style.paddingTop = 4;
            style.paddingBottom = 4;

            VisualElement statusContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                }
            };
            Add(statusContainer);

            _versionLabel = new Label
            {
                text = "Draft",
                style =
                {
                    display = DisplayStyle.None,
                    marginRight = 2,
                    paddingLeft= 2,
                    paddingRight= 2,
                    borderLeftWidth = statusLabelBorderWidth,
                    borderRightWidth = statusLabelBorderWidth,
                    borderTopWidth = statusLabelBorderWidth,
                    borderBottomWidth = statusLabelBorderWidth,
                    borderTopLeftRadius = statusLabelBorderRadius ,
                    borderTopRightRadius = statusLabelBorderRadius ,
                    borderBottomLeftRadius = statusLabelBorderRadius ,
                    borderBottomRightRadius = statusLabelBorderRadius ,
                    borderLeftColor = draftLabelBorderColor,
                    borderRightColor = draftLabelBorderColor,
                    borderTopColor = draftLabelBorderColor,
                    borderBottomColor = draftLabelBorderColor,
                    unityTextAlign = TextAnchor.MiddleCenter,
                }
            };
            statusContainer.Add(_versionLabel);

            _unreadLabel = new Label
            {
                text = "Unread",
                style =
                {
                    display = DisplayStyle.None,
                    marginLeft = 2,
                    paddingLeft= 2,
                    paddingRight= 2,
                    borderLeftWidth = statusLabelBorderWidth,
                    borderRightWidth = statusLabelBorderWidth,
                    borderTopWidth = statusLabelBorderWidth,
                    borderBottomWidth = statusLabelBorderWidth,
                    borderTopLeftRadius = statusLabelBorderRadius,
                    borderTopRightRadius = statusLabelBorderRadius,
                    borderBottomLeftRadius = statusLabelBorderRadius,
                    borderBottomRightRadius = statusLabelBorderRadius,
                    borderLeftColor = unreadLabelBorderColor,
                    borderRightColor = unreadLabelBorderColor,
                    borderTopColor = unreadLabelBorderColor,
                    borderBottomColor = unreadLabelBorderColor,
                    unityTextAlign = TextAnchor.MiddleCenter,
                }
            };
            statusContainer.Add(_unreadLabel);

            _titleLabel = new Label
            {
                text = "-",
                enableRichText = true,
                style =
                {
                    fontSize = 16,
                }
            };
#if UNITY_2022_3_OR_NEWER
            ((ITextSelection)_titleLabel).isSelectable = true;
#endif
            Add(_titleLabel);

            _authorLabel = new Label
            {
                text = "-",
                enableRichText = true,
                style =
                {
                    marginLeft = 2,
                    fontSize = 11,
                    unityFontStyleAndWeight = FontStyle.Italic,
                }
            };
#if UNITY_2022_3_OR_NEWER
            ((ITextSelection)_authorLabel).isSelectable = true;
#endif
            Add(_authorLabel);

            _historyPopup = new PopupField<long>
            {
                choices = _historyTimestamps,
                value = 0,
                formatSelectedValueCallback = Utility.FormatTimestamp,
                formatListItemCallback = Utility.FormatTimestamp,
                style =
                {
                    marginLeft = 0,
                    alignSelf = Align.FlexStart,
                }
            };
            _historyPopup.RegisterValueChangedCallback(SelectHistory);
            Add(_historyPopup);

            ScrollView contentScrollView = new ScrollView
            {
                style =
                {
                    flexGrow = 1,
                    marginTop = 10,
                }
            };
            Add(contentScrollView);
            _contentLabel = new Label
            {
                text = "-",
                enableRichText = true,
                style =
                {
                    flexGrow = 1,
                    fontSize = 14,
                    whiteSpace = WhiteSpace.Normal,
                    //borderLeftWidth = contentLabelBorderSize,
                    //borderRightWidth = contentLabelBorderSize,
                    //borderTopWidth = contentLabelBorderSize,
                    //borderBottomWidth = contentLabelBorderSize,
                    //borderTopLeftRadius = contentLabelBorderSize,
                    //borderTopRightRadius = contentLabelBorderSize,
                    //borderBottomLeftRadius = contentLabelBorderSize,
                    //borderBottomRightRadius = contentLabelBorderSize,
                    //borderLeftColor = EditorGUIUtility.isProSkin ? contentLabelBorderColorDark : contentLabelBorderColorLight,
                    //borderRightColor = EditorGUIUtility.isProSkin ? contentLabelBorderColorDark : contentLabelBorderColorLight,
                    //borderTopColor = EditorGUIUtility.isProSkin ? contentLabelBorderColorDark : contentLabelBorderColorLight,
                    //borderBottomColor = EditorGUIUtility.isProSkin ? contentLabelBorderColorDark : contentLabelBorderColorLight,
                }
            };
#if UNITY_2022_3_OR_NEWER
            ((ITextSelection)_contentLabel).isSelectable = true;
#endif
            contentScrollView.Add(_contentLabel);

            VisualElement opContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.FlexEnd,
                    minHeight= 22,
                    maxHeight= 22,
                    marginBottom = 2,
                },
            };
            Add(opContainer);

            _markButton = new Button(MarkStatus)
            {
                text = "-",
                style =
                {
                    width = 110,
                    marginRight = 0,
                    paddingRight = 0,
                    borderTopLeftRadius = Utility.ButtonBorderRadius,
                    borderTopRightRadius = Utility.ButtonBorderRadius,
                    borderBottomLeftRadius = Utility.ButtonBorderRadius,
                    borderBottomRightRadius = Utility.ButtonBorderRadius,
                }
            };
            opContainer.Add(_markButton);

            _opDropdownButton = new Button(CreateOpDropdownMenu)
            {
                style =
                {
                    backgroundImage = EditorGUIUtility.isProSkin
                        ? EditorGUIUtility.Load("d_icon dropdown@2x") as Texture2D
                        : EditorGUIUtility.Load("icon dropdown@2x") as Texture2D,
                    width = 14,
                    marginLeft = 0,
                    paddingLeft = 0,
                    borderTopLeftRadius = Utility.ButtonBorderRadius,
                    borderTopRightRadius = Utility.ButtonBorderRadius,
                    borderBottomLeftRadius = Utility.ButtonBorderRadius,
                    borderBottomRightRadius = Utility.ButtonBorderRadius,
                }
            };
            opContainer.Add(_opDropdownButton);
        }

        public void SetNote(NoteEntry note)
        {
            _note = note;
            _versionLabel.text = _note?.isDraft ?? false
                ? "Draft"
                : null;
            _versionLabel.style.display = _note?.isDraft ?? false
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            _unreadLabel.style.display = _note != null && ProjectNotesLocalCache.instance.IsUnread(_note.GetKey())
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            _titleLabel.text = _note?.title ?? "TITLE";
            _authorLabel.text = _note == null ? "AUTHOR" : $"by {_note.author}";
            _note.CollectHistoryTimestamps(_historyTimestamps);
            _historyPopup.SetValueWithoutNotify(_note?.timestamp ?? 0L);
            _contentLabel.text = _note?.content ?? "CONTENT";
            UpdateOpButtons();
        }

        public void RefreshView()
        {
            SetNote(_note);
        }

        private void CreateOpDropdownMenu()
        {
            if (_note == null)
            {
                return;
            }

            GenericMenu menu = new GenericMenu();
            if (_note.timestamp == _historyPopup.value)
            {
                menu.AddItem(new GUIContent("Edit"), false, OnWantsToEditNote);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, OnWantsToDeleteNote);
                menu.DropDown(_opDropdownButton.worldBound);
            }
            else
            {
                menu.AddItem(new GUIContent("Delete"), false, OnWantsToDeleteNote);
                menu.DropDown(_opDropdownButton.worldBound);
            }
        }

        private void SelectHistory(ChangeEvent<long> evt)
        {
            long timestamp = evt.newValue;
            if (timestamp == _note.timestamp)
            {
                RefreshView();
                return;
            }

            foreach (NoteHistory history in _note.contentHistory)
            {
                if (history.timestamp == timestamp)
                {
                    _versionLabel.text = "Old Version";
                    _versionLabel.style.display = DisplayStyle.Flex;
                    _unreadLabel.style.display = DisplayStyle.None;
                    _contentLabel.text = history.content;
                    UpdateOpButtons();
                    return;
                }
            }

            UDebug.LogError($"[Project Notes] Unable to find historical version for timestamp '{timestamp}'.");
            RefreshView();
        }

        private void UpdateOpButtons()
        {
            _markButton.text = _note == null || _note.timestamp != _historyPopup.value
                ? "MARK"
                : ProjectNotesLocalCache.instance.IsUnread(_note.GetKey())
                    ? "Mark as Read"
                    : "Mark as Unread";
            _markButton.SetEnabled(_note != null && _note.timestamp == _historyPopup.value);
            _opDropdownButton.SetEnabled(_note != null);
        }

        private void MarkStatus()
        {
            if (_note == null)
            {
                return;
            }

            bool unread = ProjectNotesLocalCache.instance.IsUnread(_note.GetKey());
            if (unread)
            {
                ProjectNotesLocalCache.instance.MarkAsRead(_note.GetKey());
            }
            else
            {
                ProjectNotesLocalCache.instance.MarkAsUnread(_note.GetKey());
            }

            RefreshView();

            readStatusChanged?.Invoke(_note);
        }

        private void OnWantsToEditNote()
        {
            if (_note == null)
            {
                return;
            }

            wantsToEditNote?.Invoke(_note);
        }

        private void OnWantsToDeleteNote()
        {
            if (_note == null)
            {
                return;
            }

            string message = $"{_note.title}\n\n" +
                $"This operation cannot be undone.\n" +
                $"Once the settings is synced to the version control system, this note will be removed from the project of all team members.";
            if (EditorUtility.DisplayDialog("Delete selected note?", message, "Delete", "Cancel"))
            {
                wantsToDeleteNote?.Invoke(_note, _historyPopup.value);
            }
        }
    }
}