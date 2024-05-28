using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class NoteEditWindow : EditorWindow
    {
        public static NoteEditWindow Open(NoteEntry note, Action<NoteEntry, bool> onSave)
        {
            _note = note;
            _onSave = onSave;
            NoteEditWindow window = GetWindow<NoteEditWindow>(true);
            //window.ShowModalUtility();
            return window;
        }


        private static NoteEntry _note;
        // param: note, isNewNote
        private static Action<NoteEntry, bool> _onSave;

        public TextField _guidField;
        public LongField _timestampField;
        public TextField _categoryField;
        public TextField _authorField;
        public Toggle _draftField;
        public IntegerField _priorityField;
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
                value = _note?.categoryTrimmed,
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

            _priorityField = new IntegerField("Priority")
            {
                value = _note?.priority ?? 0,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _priorityField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_priorityField);

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

            Button submitButton = new Button(SaveAndClose)
            {
                text = "Save",
                style =
                {
                    alignSelf = Align.FlexEnd,
                    marginTop = 10,
                    marginBottom = 4,
                    marginLeft = 4,
                    marginRight = 4,
                    height = 24,
                    width = 60,
                },
            };
            root.Add(submitButton);
        }

        private void OnDisable()
        {
            _note = null;
            _onSave = null;
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

        private void SaveAndClose()
        {
            if (_onSave == null)
            {
                Close();
                return;
            }

            bool isNewNote = _note == null;
            string message = isNewNote
                ? "Once the settings is synced to the version control system, this note will be added to the project of all team members."
                : "Once the settings is synced to the version control system, this note will be updated in the project of all team members.";
            if (!EditorUtility.DisplayDialog("Save note content?", message, "Save", "Cancel"))
            {
                return;
            }

            Action<NoteEntry, bool> onSubmit = _onSave;
            NoteEntry note = new NoteEntry
            {
                guid = _guidField.value,
                timestamp = _timestampField.value,
                category = _categoryField.value?.Trim(),
                author = _authorField.value,
                isDraft = _draftField.value,
                priority = _priorityField.value,
                title = _titleField.value,
                content = _contentField.value,
            };

            Close();
            onSubmit(note, isNewNote);
        }
    }
}