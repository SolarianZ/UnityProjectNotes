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


        public long VersionGuid => _versionGuid;
        public List<ProjectNoteItem> Notes => _notes;

        [SerializeField]
        private long _versionGuid = ProjectNoteUtility.NewGuid();
        [SerializeField]
        private List<ProjectNoteItem> _notes = new List<ProjectNoteItem>();


        public List<string> CollectCategories(bool addCategoryAll = true)
        {
            HashSet<string> categoriesHashSet = new HashSet<string>();
            foreach (ProjectNoteItem note in _notes)
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

        [ContextMenu("[Debug] Generate New Version Guid")]
        public void UpdateVersionGuid()
        {
            _versionGuid = ProjectNoteUtility.NewGuid();
            ForceSave();
        }

        [ContextMenu("Force Save")]
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
    }
}