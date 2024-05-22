using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class ProjectNoteContentView : VisualElement
    {
        private readonly Label _titleLabel;
        private readonly Label _contentLabel;
        private ProjectNoteItem _note;


        public ProjectNoteContentView()
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

            _contentLabel = new Label // TODO: Put int ScrollView
            {
                text = "-",
                enableRichText = true,
                style =
                {
                    flexGrow = 1,
                    whiteSpace = WhiteSpace.Normal,
                }
            };
#if UNITY_2022_3_OR_NEWER
            ((ITextSelection)_contentLabel).isSelectable = true;
#endif
            Add(_contentLabel);

            Button markButton = new Button(MarkStatus)
            {
                text = "-",
                style =
                {
                    alignSelf = Align.FlexEnd,
                    marginBottom = 5,
                    width = 110,
                }
            };
            Add(markButton);
        }

        public void SetNote(ProjectNoteItem note)
        {
            _note = note;
            _contentLabel.text = _note?.content;
        }

        private void MarkStatus()
        {
            Debug.LogError("TODO: MarkStatus");
        }
    }
}