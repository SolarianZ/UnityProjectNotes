using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UDebug = UnityEngine.Debug;

namespace GBG.ProjectNotes.Editor
{
    public class ProjectNotesSettings : ScriptableObject
    {
        #region Static

        public const string CategoryAll = "All";

        private static ProjectNotesSettings _instance;
        public static ProjectNotesSettings instance
        {
            get
            {
                if (!_instance)
                {
                    // "Resources.FindObjectsOfTypeAll<T>" can not find assets that have not been loaded.
                    string[] guids = AssetDatabase.FindAssets($"t:{nameof(ProjectNotesSettings)}");
                    if (guids.Length == 0)
                    {
                        return null;
                    }

                    _instance = AssetDatabase.LoadAssetAtPath<ProjectNotesSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
                    if (guids.Length > 1)
                    {
                        UDebug.LogWarning($"[Project Notes] Multiple {nameof(ProjectNotesSettings)} instances found, using the first one. Please remove duplicates.", _instance);
                    }
                }

                return _instance;
            }
        }

        internal static void CreateNewInstance()
        {
            string savePath = EditorUtility.SaveFilePanelInProject("Save Project Notes Settings",
                "ProjectNotesSettings", "asset", "Save Project Notes Settings");
            if (string.IsNullOrEmpty(savePath))
            {
                return;
            }

            ProjectNotesSettings instance = CreateInstance<ProjectNotesSettings>();
            AssetDatabase.CreateAsset(instance, savePath);
            EditorGUIUtility.PingObject(instance);
            UDebug.Log($"[Project Notes] Created new {nameof(ProjectNotesSettings)} asset at {savePath}.", instance);
        }

        #endregion


        public long VersionTimestamp => _versionTimestamp;
        public List<NoteEntry> Notes => _notes;

        [SerializeField]
        private long _versionTimestamp = Utility.NewTimestamp();
        [SerializeField]
        private List<NoteEntry> _notes = new List<NoteEntry>();


        public List<string> CollectCategories(bool addCategoryAll = true)
        {
            HashSet<string> categoriesHashSet = new HashSet<string>();
            foreach (NoteEntry note in _notes)
            {
                string category = note.category?.Trim();
                if (string.IsNullOrEmpty(category))
                {
                    continue;
                }

                if (category == CategoryAll)
                {
                    continue;
                }

                categoriesHashSet.Add(category);
            }

            List<string> categories = new List<string>(categoriesHashSet.Count + 1);
            if (addCategoryAll)
            {
                categories.Add(CategoryAll);
            }
            categories.AddRange(categoriesHashSet);
            return categories;
        }

        [ContextMenu("Force Save", false, 0)]
        internal void ForceSave()
        {
            EditorUtility.SetDirty(this);
#if !UNITY_6000_0_OR_NEWER
            // MEMO Unity Bug: https://issuetracker.unity3d.com/product/unity/issues/guid/UUM-66169
            // Fixed in 2022.3.29f1, 6000.0.0b16
            AssetDatabase.MakeEditable(AssetDatabase.GetAssetPath(this));
#endif
            AssetDatabase.SaveAssetIfDirty(this);
        }

        [ContextMenu("[Debug] Generate New Guid to Clipboard", false, 100)]
        public void GenerateNewGuidToClipboard()
        {
            GUIUtility.systemCopyBuffer = Utility.NewGuid();
        }

        [ContextMenu("[Debug] Generate New Timestamp to Clipboard", false, 101)]
        public void GenerateNewTimestampToClipboard()
        {
            GUIUtility.systemCopyBuffer = Utility.NewTimestamp().ToString();
        }
    }
}