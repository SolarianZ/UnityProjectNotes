using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class NoteContentView : VisualElement
    {
        private readonly Label _titleLabel;
        private readonly Label _authorLabel;
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

            VisualElement titleContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                }
            };
            Add(titleContainer);

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
            titleContainer.Add(_titleLabel);

            DropdownField historyDropdown = new DropdownField
            {
                style =
                {
                    marginLeft = 0,
                }
            };
            titleContainer.Add(historyDropdown);

            _authorLabel = new Label
            {
                text = "-",
                enableRichText = true,
                style =
                {
                    fontSize = 11,
                    unityFontStyleAndWeight = FontStyle.Italic,
                }
            };
#if UNITY_2022_3_OR_NEWER
            ((ITextSelection)_authorLabel).isSelectable = true;
#endif
            Add(_authorLabel);

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
                    marginBottom = 5,
                    width = 110,
                }
            };
            Add(_markButton);
        }

        public void SetNote(NoteEntry note)
        {
            _note = note;
            _titleLabel.text = _note?.title ?? "TITLE";
            _authorLabel.text = _note == null ? "AUTHOR" : $"by {_note.author}";
            _contentLabel.text = _note?.content ?? "CONTENT";
            UpdateMarkButtonText();
            _markButton.SetEnabled(_note != null);
        }

        public void RefreshView()
        {
            SetNote(_note);
        }

        private void UpdateMarkButtonText()
        {
            _markButton.text = _note == null
                ? "MARK"
                : ProjectNotesLocalCache.instance.IsRead(_note.GetKey())
                    ? "Mark as Unread"
                    : "Mark as Read";
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

            UpdateMarkButtonText();

            readStatusChanged?.Invoke(_note);
        }
    }
}