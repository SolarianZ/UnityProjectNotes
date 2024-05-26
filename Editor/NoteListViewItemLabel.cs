using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class NoteListViewItemLabel : Label
    {
        public NoteEntry Note { get; private set; }
        private Image _redDotIcon;

        public bool redDotIconVisible
        {
            get
            {
                return _redDotIcon != null
                    ? _redDotIcon.style.display == DisplayStyle.Flex
                    : false;
            }
            set
            {
                if (value)
                {
                    if (_redDotIcon == null)
                    {
                        CreateRedDotIcon();
                    }

                    _redDotIcon.style.display = DisplayStyle.Flex;
                }
                else if (_redDotIcon != null)
                {
                    _redDotIcon.style.display = DisplayStyle.None;
                }
            }
        }


        public NoteListViewItemLabel()
        {
            style.paddingLeft = 4;
            style.paddingRight = 4;
            style.unityTextAlign = TextAnchor.MiddleLeft;
        }

        private void CreateRedDotIcon()
        {
            _redDotIcon = new Image
            {
                image = EditorGUIUtility.Load(Utility.RedDotIconName) as Texture,
                style =
                {
                    alignSelf = Align.FlexEnd,
                    marginTop = 2,
                    width = 8,
                    height = 8,
                }
            };
            Add(_redDotIcon);
        }

        public void Bind(NoteEntry note)
        {
            Note = note;
            UpdateView();
        }

        public void Unbind()
        {
            Note = null;
            UpdateView();
        }

        public void UpdateView()
        {
            if (Note == null)
            {
                text = null;
                redDotIconVisible = false;
            }
            else
            {
                text = Note.title;
                bool unread = ProjectNotesLocalCache.instance.IsUnread(Note.GetKey());
                redDotIconVisible = unread;
            }
        }


        public static VisualElement MakeItem()
        {
            NoteListViewItemLabel itemView = new NoteListViewItemLabel();
            return itemView;
        }
    }
}