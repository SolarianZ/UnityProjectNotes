#if !GBG_PROJECTNOTES_DEV
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    [CustomEditor(typeof(ProjectNotesSettings))]
    public class ProjectNotesSettingsEditor : UnityEditor.Editor
    {
        // TODO FIXME: ObjectDisposedException thrown on reset asset:
        // ObjectDisposedException: SerializedProperty _notes.Array.data[0].content has disappeared!
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            InspectorElement.FillDefaultInspector(container, serializedObject, this);
            container.SetEnabled(false);
            return container;
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                base.OnInspectorGUI();
            }
        }
    }
} 
#endif