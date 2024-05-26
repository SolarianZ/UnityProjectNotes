using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class NoteEditWindow : EditorWindow
    {
        public static NoteEditWindow Open(NoteEntry note, Action<NoteEntry> onSubmit)
        {
            _note = note;
            _onSubmit = onSubmit;
            NoteEditWindow window = GetWindow<NoteEditWindow>(true);
            //window.ShowModalUtility();
            return window;
        }


        private static NoteEntry _note;
        private static Action<NoteEntry> _onSubmit;

        public TextField _guidField;
        public LongField _timestampField;
        public TextField _categoryField;
        public TextField _authorField;
        public Toggle _draftField;
        public TextField _titleField;
        public TextField _contentField;


        private void OnEnable()
        {
            SetWindowTitle();
            minSize = new Vector2(400, 360);
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            ScrollView scrollView = new ScrollView
            {
                style = { flexGrow = 1 },
            };
            root.Add(scrollView);

            const float LabelWidth = 70;
            _guidField = new TextField("Guid")
            {
                value = _note?.guid ?? Utility.NewGuid(),
                isReadOnly = true,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _guidField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_guidField);

            _timestampField = new LongField("Timestamp")
            {
                value = Utility.NewTimestamp(),
                isReadOnly = true,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _timestampField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_timestampField);

            _categoryField = new TextField("Category")
            {
                value = _note?.category,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _categoryField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_categoryField);

            _authorField = new TextField("Author")
            {
                value = _note?.author ?? Environment.UserName,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _authorField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_authorField);

            _draftField = new Toggle("Draft")
            {
                value = _note?.isDraft ?? false,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _draftField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_draftField);

            _titleField = new TextField("Title")
            {
                value = _note?.title,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _titleField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_titleField);

            _contentField = new TextField("Content")
            {
                value = _note?.content,
                multiline = true,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            Label contentFieldLabel = _contentField.Q<Label>();
            contentFieldLabel.style.minWidth = LabelWidth;
            contentFieldLabel.style.alignSelf = Align.FlexStart;
            _contentField.Q(name: "unity-text-input").style.minHeight = 180;
            scrollView.Add(_contentField);

            Button submitButton = new Button(SubmitAndClose)
            {
                text = GetSubmitButtonText(),
                style =
                {
                    alignSelf = Align.FlexEnd,
                    marginTop = 10,
                    marginBottom = 4,
                    marginLeft = 4,
                    marginRight = 4,
                    minHeight = 30,
                    maxHeight = 30,
                    width = 100,
                },
            };
            root.Add(submitButton);
        }

        private void SetWindowTitle()
        {
            if (_note == null)
            {
                titleContent = new GUIContent("Add Note");
            }
            else
            {
                titleContent = new GUIContent("Edit Note");
            }
        }

        private string GetSubmitButtonText()
        {
            if (_note == null)
            {
                return "Add";
            }
            else
            {
                return "Save";
            }
        }

        private void SubmitAndClose()
        {
            if (_onSubmit == null)
            {
                _note = null;
                Close();
                return;
            }

            NoteEntry note = new NoteEntry
            {
                guid = _guidField.value,
                timestamp = _timestampField.value,
                category = _categoryField.value,
                author = _authorField.value,
                isDraft = _draftField.value,
                title = _titleField.value,
                content = _contentField.value,
            };

            Close();

            Action<NoteEntry> onSubmit = _onSubmit;
            _note = null;
            _onSubmit = null;
            onSubmit(note);
        }
    }
}