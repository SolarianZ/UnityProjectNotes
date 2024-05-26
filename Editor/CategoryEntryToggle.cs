using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class CategoryEntryToggle : ToolbarToggle
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


        public CategoryEntryToggle(string text = null)
        {
            this.text = text;
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
                    width = 8,
                    height = 8,
                }
            };
            Add(_redDotIcon);
        }
    }
}