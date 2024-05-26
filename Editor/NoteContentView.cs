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
            _titleLabel.text = _note?.title ?? "TITLE";
            _authorLabel.text = _note == null ? "AUTHOR" : $"by {_note.author}";
            Utility.CollectHistoryTimestamps(_note, _historyTimestamps);
            _historyPopup.value = _note?.timestamp ?? 0L;
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
                _contentLabel.text = _note.content;
                UpdateMarkButton();
                return;
            }

            foreach (NoteHistory history in _note.contentHistory)
            {
                if (history.timestamp == timestamp)
                {
                    _contentLabel.text = history.content;
                    UpdateMarkButton();
                    return;
                }
            }

            UDebug.LogError($"Unable to find historical version for timestamp '{timestamp}'.");
            _historyPopup.index = 0;
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

            UpdateMarkButton();

            readStatusChanged?.Invoke(_note);
        }
    }
}