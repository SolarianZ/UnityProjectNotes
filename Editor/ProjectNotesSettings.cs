using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UDebug = UnityEngine.Debug;

namespace GBG.ProjectNotes.Editor
{

    public class ProjectNotesSettings : ScriptableObject
    {
        #region Static

        private static ProjectNotesSettings _instance;
        public static ProjectNotesSettings instance
        {
            get
            {
                if (!_instance)
                {
                    ProjectNotesSettings[] instances = Resources.FindObjectsOfTypeAll<ProjectNotesSettings>();
                    if (instances.Length == 0)
                    {
                        return null;
                    }

                    _instance = instances[0];
                    if (instances.Length > 1)
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
            UDebug.Log($"[Project Notes] Created new {nameof(ProjectNotesSettings)} asset at {savePath}.", instance);
        }

        #endregion


        public string VersionGuid => _versionGuid;
        public List<ProjectNoteItem> Notes => _notes;

        [SerializeField]
        private string _versionGuid = Guid.NewGuid().ToString();
        [SerializeField]
        private List<ProjectNoteItem> _notes = new List<ProjectNoteItem>();

        [ContextMenu("[Debug] Generate New Version Guid")]
        public void UpdateVersionGuid()
        {
            _versionGuid = Guid.NewGuid().ToString();
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