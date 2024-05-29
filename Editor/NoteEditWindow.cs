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
            //window.ShowModalUtility(); Conflict with undo/redo
            return window;
        }

        //private static NoteEditWindow _instance;

        [Flags]
        enum InvalidStatus
        {
            Category = 1 << 0,
            Author = 1 << 1,
            Title = 1 << 2,
            Content = 1 << 3,
        }


        public delegate void SaveNoteHandler(NoteEntry note, bool isNewNote);

        public const float FieldLabelWidth = 70;
        public const float AlertLabelOffset = 80;
        public static Color WarningTextColor = Color.yellow;// new Color32(255, 180, 0, 255);
        public static Color ErrorTextColor = new Color32(240, 0, 0, 255);

        private SaveNoteHandler _onSave;
        private bool _isNewNote;
        private InvalidStatus _invalidStatus;
        [SerializeField]
        private NoteEntry _serializedNote; // for undo/redo use
        private SerializedObject _serializedObject;

        private TextField _guidField;
        private TextField _timestampField;
        private TextField _categoryField;
        private Label _categoryAlertLabel;
        private TextField _authorField;
        private Label _authorAlertLabel;
        private Toggle _draftField;
        private IntegerField _priorityField;
        private TextField _titleField;
        private Label _titleAlertLabel;
        private TextField _contentField;
        private Label _contentAlertLabel;


        private void OnEnable()
        {
            //_instance = this;

            SetWindowTitle();
            minSize = new Vector2(400, 360);

            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            ScrollView scrollView = new ScrollView
            {
                style = { flexGrow = 1 },
            };
            root.Add(scrollView);

            _guidField = new TextField("Guid")
            {
                isReadOnly = true,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _guidField.Q<Label>().style.minWidth = FieldLabelWidth;
            scrollView.Add(_guidField);

            _timestampField = new TextField("Timestamp")
            {
                isReadOnly = true,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _timestampField.Q<Label>().style.minWidth = FieldLabelWidth;
            scrollView.Add(_timestampField);

            _categoryField = new TextField("Category")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.category)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _categoryField.RegisterValueChangedCallback(OnCategoryChanged);
            _categoryField.Q<Label>().style.minWidth = FieldLabelWidth;
            scrollView.Add(_categoryField);
            _categoryAlertLabel = CreateAlertLabel();
            scrollView.Add(_categoryAlertLabel);

            _authorField = new TextField("Author")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.author)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _authorField.RegisterValueChangedCallback(OnAuthorChanged);
            _authorField.Q<Label>().style.minWidth = FieldLabelWidth;
            scrollView.Add(_authorField);
            _authorAlertLabel = CreateAlertLabel();
            scrollView.Add(_authorAlertLabel);

            _draftField = new Toggle("Draft")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.isDraft)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _draftField.RegisterValueChangedCallback(OnDraftStatusChanged);
            _draftField.Q<Label>().style.minWidth = FieldLabelWidth;
            scrollView.Add(_draftField);

            _priorityField = new IntegerField("Priority")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.priority)}",
                tooltip = "High priority categories and entries will be listed first.",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _priorityField.RegisterValueChangedCallback(OnPriorityChanged);
            _priorityField.Q<Label>().style.minWidth = FieldLabelWidth;
            scrollView.Add(_priorityField);

            _titleField = new TextField("Title")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.title)}",
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _titleField.RegisterValueChangedCallback(OnTitleChanged);
            _titleField.Q<Label>().style.minWidth = FieldLabelWidth;
            scrollView.Add(_titleField);
            _titleAlertLabel = CreateAlertLabel();
            scrollView.Add(_titleAlertLabel);

            _contentField = new TextField("Content")
            {
                bindingPath = $"{nameof(_serializedNote)}.{nameof(_serializedNote.content)}",
                multiline = true,
                style = { unityTextAlign = TextAnchor.MiddleRight, },
            };
            _contentField.RegisterValueChangedCallback(OnContentChanged);
            Label contentFieldLabel = _contentField.Q<Label>();
            contentFieldLabel.style.minWidth = FieldLabelWidth;
            contentFieldLabel.style.alignSelf = Align.FlexStart;
            _contentField.Q(name: "unity-text-input").style.minHeight = 180;
            scrollView.Add(_contentField);
            _contentAlertLabel = CreateAlertLabel();
            scrollView.Add(_contentAlertLabel);

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
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;

            _onSave = null;
            _serializedObject?.Dispose();
            _serializedObject = null;
        }

        //private void OnLostFocus()
        //{
        //    if (_instance)
        //    {
        //        _instance.Focus();
        //        GUIContent message = EditorGUIUtility.IconContent("Warning@2x");
        //        message.text = "Please close the note editing window\n" +
        //                       "before switching to other windows.";
        //        ShowNotification(message, 2);
        //    }
        //}

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

            _serializedObject?.Dispose();
            _serializedObject = new SerializedObject(this);
            rootVisualElement.Bind(_serializedObject);

            SetWindowTitle();
            ValidateAll();
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

        private Label CreateAlertLabel()
        {
            Label label = new Label
            {
                style =
                {
                    display = DisplayStyle.None,
                    marginLeft = AlertLabelOffset,
                    fontSize = 11,
                    unityFontStyleAndWeight = FontStyle.Italic,
                },
            };
            return label;
        }

        private void OnBeforeAssemblyReload()
        {
            Close();
        }


        #region Validation

        private void ValidateAll()
        {
            ValidateCategory();
            ValidateAuthor();
            ValidateTitle();
            ValidateContent();
        }

        private void OnCategoryChanged(ChangeEvent<string> evt)
        {
            ValidateCategory();
        }

        private bool ValidateCategory()
        {
            if (string.IsNullOrWhiteSpace(_serializedNote.category))
            {
                _invalidStatus |= InvalidStatus.Category;
                _categoryAlertLabel.text = "The category cannot be empty.";
                _categoryAlertLabel.style.color = ErrorTextColor;
                _categoryAlertLabel.style.display = DisplayStyle.Flex;
                return false;
            }

            _invalidStatus &= ~InvalidStatus.Category;
            _categoryAlertLabel.style.display = DisplayStyle.None;
            return true;
        }

        private void OnAuthorChanged(ChangeEvent<string> evt)
        {
            ValidateAuthor();
        }

        private bool ValidateAuthor()
        {
            if (string.IsNullOrWhiteSpace(_serializedNote.author))
            {
                _invalidStatus |= InvalidStatus.Author;
                _authorAlertLabel.text = "The author cannot be empty.";
                _authorAlertLabel.style.color = ErrorTextColor;
                _authorAlertLabel.style.display = DisplayStyle.Flex;
                return false;
            }

            _invalidStatus &= ~InvalidStatus.Author;
            _authorAlertLabel.style.display = DisplayStyle.None;
            return true;
        }

        private void OnDraftStatusChanged(ChangeEvent<bool> evt)
        {
        }

        private void OnPriorityChanged(ChangeEvent<int> evt)
        {
        }

        private void OnTitleChanged(ChangeEvent<string> evt)
        {
            ValidateTitle();
        }

        private bool ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(_serializedNote.title))
            {
                _invalidStatus |= InvalidStatus.Title;
                _titleAlertLabel.text = "The title cannot be empty.";
                _titleAlertLabel.style.color = ErrorTextColor;
                _titleAlertLabel.style.display = DisplayStyle.Flex;
                return false;
            }

            _invalidStatus &= ~InvalidStatus.Title;
            Utility.IsNiceTitle(_serializedNote.title, out string alert);
            if (string.IsNullOrEmpty(alert))
            {
                _titleAlertLabel.style.display = DisplayStyle.None;
            }
            else
            {
                _titleAlertLabel.text = alert;
                _titleAlertLabel.style.color = WarningTextColor;
                _titleAlertLabel.style.display = DisplayStyle.Flex;
            }

            return true;
        }

        private void OnContentChanged(ChangeEvent<string> evt)
        {
            ValidateContent();
        }

        private bool ValidateContent()
        {
            if (string.IsNullOrWhiteSpace(_serializedNote.content))
            {
                _invalidStatus |= InvalidStatus.Content;
                _contentAlertLabel.text = "The content cannot be empty.";
                _contentAlertLabel.style.color = ErrorTextColor;
                _contentAlertLabel.style.display = DisplayStyle.Flex;
                return false;
            }

            _invalidStatus &= ~InvalidStatus.Content;
            Utility.IsNiceContent(_serializedNote.content, out string alert);
            if (string.IsNullOrEmpty(alert))
            {
                _contentAlertLabel.style.display = DisplayStyle.None;
            }
            else
            {
                _contentAlertLabel.text = alert;
                _contentAlertLabel.style.color = WarningTextColor;
                _contentAlertLabel.style.display = DisplayStyle.Flex;
            }

            return true;
        }

        #endregion


        private void SaveAndClose()
        {
            if (_onSave == null)
            {
                Close();
                return;
            }

            if (_invalidStatus != 0)
            {
                EditorUtility.DisplayDialog("Unable to save note", "Please fill in all required fields before saving.", "Ok");
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