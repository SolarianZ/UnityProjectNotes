using System;
using System.Collections.Generic;
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

        private readonly Label _versionLabel;
        private readonly Label _unreadLabel;
        private readonly Label _titleLabel;
        private readonly Label _authorLabel;
        private readonly PopupField<long> _historyPopup;
        private readonly List<long> _historyTimestamps = new List<long>();
        private readonly Label _contentLabel;
        private readonly Button _markButton;
        private NoteEntry _note;

        public event Action<NoteEntry> readStatusChanged;


        public NoteContentView()
        {
            style.paddingLeft = 8;
            style.paddingRight = 4;
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
                }
            };
#if UNITY_2022_3_OR_NEWER
            ((ITextSelection)_contentLabel).isSelectable = true;
#endif
            contentScrollView.Add(_contentLabel);

            _markButton = new Button(MarkStatus)
            {
                text = "-",
                style =
                {
                    alignSelf = Align.FlexEnd,
                    width = 110,
                    marginBottom = 5,
                    borderTopLeftRadius = 2,
                    borderTopRightRadius = 2,
                    borderBottomLeftRadius = 2,
                    borderBottomRightRadius = 2,
                }
            };
            Add(_markButton);
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
            _unreadLabel.style.display = _note != null && !ProjectNotesLocalCache.instance.IsRead(_note.GetKey())
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            _titleLabel.text = _note?.title ?? "TITLE";
            _authorLabel.text = _note == null ? "AUTHOR" : $"by {_note.author}";
            Utility.CollectHistoryTimestamps(_note, _historyTimestamps);
            _historyPopup.SetValueWithoutNotify(_note?.timestamp ?? 0L);
            _contentLabel.text = _note?.content ?? "CONTENT";
            UpdateMarkButton();
        }

        public void RefreshView()
        {
            SetNote(_note);
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
                    UpdateMarkButton();
                    return;
                }
            }

            UDebug.LogError($"Unable to find historical version for timestamp '{timestamp}'.");
            RefreshView();
        }

        private void UpdateMarkButton()
        {
            _markButton.text = _note == null || _note.timestamp != _historyPopup.value
                ? "MARK"
                : ProjectNotesLocalCache.instance.IsRead(_note.GetKey())
                    ? "Mark as Unread"
                    : "Mark as Read";
            _markButton.SetEnabled(_note != null && _note.timestamp == _historyPopup.value);
        }

        private void MarkStatus()
        {
            if (_note == null)
            {
                return;
            }

            bool read = ProjectNotesLocalCache.instance.IsRead(_note.GetKey());
            if (read)
            {
                ProjectNotesLocalCache.instance.MarkAsUnread(_note.GetKey());
            }
            else
            {
                ProjectNotesLocalCache.instance.MarkAsRead(_note.GetKey());
            }

            RefreshView();

            readStatusChanged?.Invoke(_note);
        }
    }
}