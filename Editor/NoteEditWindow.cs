using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class NoteEditWindow : EditorWindow
    {
        public static NoteEditWindow Open(NoteEntry note, SaveNoteHandler onSave)
        {
            NoteEditWindow window = GetWindow<NoteEditWindow>(true);
            window.Initialize(note, onSave); // Called after CreateGUI
            //window.ShowModalUtility();
            return window;
        }

        public delegate void SaveNoteHandler(NoteEntry note, bool isNewNote);

        private SaveNoteHandler _onSave;
        private bool _isNewNote;
        [SerializeField]
        private NoteEntry _serializedNote; // for undo/redo use
        private SerializedObject _serializedObject;

        private TextField _guidField;
        private TextField _timestampField;
        private TextField _categoryField;
        private TextField _authorField;
        private Toggle _draftField;
        private IntegerField _priorityField;
        private TextField _titleField;
        private TextField _contentField;


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
                isReadOnly = true,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _guidField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_guidField);

            //_timestampField = new LongField("Timestamp")
            _timestampField = new TextField("Timestamp")
            {
                isReadOnly = true,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _timestampField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_timestampField);

            _categoryField = new TextField("Category")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.category)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _categoryField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_categoryField);

            _authorField = new TextField("Author")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.author)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _authorField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_authorField);

            _draftField = new Toggle("Draft")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.isDraft)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _draftField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_draftField);

            _priorityField = new IntegerField("Priority")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.priority)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _priorityField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_priorityField);

            _titleField = new TextField("Title")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.title)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _titleField.Q<Label>().style.minWidth = LabelWidth;
            scrollView.Add(_titleField);

            _contentField = new TextField("Content")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.content)}",
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
            _onSave = null;
            _serializedObject?.Dispose();
            _serializedObject = null;
        }

        private void Update()
        {
            if (_timestampField != null && _serializedNote != null)
            {
                DateTime now = DateTime.Now;
                _serializedNote.timestamp = now.Ticks;
                _timestampField.SetValueWithoutNotify(now.ToString(Utility.DateTimeFormat));
            }
        }

        private void Initialize(NoteEntry srcNote, SaveNoteHandler onSave)
        {
            _isNewNote = srcNote == null;
            _onSave = onSave;
            _serializedNote ??= new NoteEntry();
            if (srcNote != null)
            {
                _serializedNote.CopyFrom(srcNote);
            }
            else
            {
                _serializedNote.guid = Utility.NewGuid();
                _serializedNote.author = Environment.UserName;
            }
            _serializedNote.timestamp = Utility.NewTimestamp();
            _guidField.SetValueWithoutNotify(_serializedNote.guid);
            _timestampField.SetValueWithoutNotify(Utility.FormatTimestamp(_serializedNote.timestamp));

            _serializedObject = new SerializedObject(this);
            rootVisualElement.Bind(_serializedObject);

            SetWindowTitle();
        }

        private void SetWindowTitle()
        {
            if (_isNewNote)
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

            string message = _isNewNote
                ? "Once the settings is synced to the version control system, this note will be added to the project of all team members."
                : "Once the settings is synced to the version control system, this note will be updated in the project of all team members.";
            if (!EditorUtility.DisplayDialog("Save note content?", message, "Save", "Cancel"))
            {
                return;
            }

            SaveNoteHandler onSubmit = _onSave;
            NoteEntry note = new NoteEntry();
            note.CopyFrom(_serializedNote);

            Close();

            onSubmit(note, _isNewNote);
        }
    }
}