using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class ProjectNoteListItemLabel : Label
    {
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
                if (visible)
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


        public ProjectNoteListItemLabel()
        {
            style.paddingLeft = 4;
            style.paddingRight = 4;
            style.unityTextAlign = TextAnchor.MiddleLeft;

            CreateRedDotIcon();
        }

        private void CreateRedDotIcon()
        {
            _redDotIcon = new Image
            {
                //image = EditorGUIUtility.Load("winbtn_mac_close") as Texture,
                image = EditorGUIUtility.Load("redLight") as Texture,
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

        public void SetupView(string title, bool unread)
        {
            text = title;
            redDotIconVisible = unread;
        }


        public static VisualElement MakeItem()
        {
            ProjectNoteListItemLabel itemView = new ProjectNoteListItemLabel();
            return itemView;
        }
    }
}