﻿using System.Collections.Generic;
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
                        UDebug.LogError($"[Project Notes] Multiple {nameof(ProjectNotesSettings)} instances found, using the first one. Please remove duplicates.", _instance);
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
        private List<NoteEntry> _notes = new List<NoteEntry>
        {
            new NoteEntry
            {
                category = "Sample",
                author = "ZQY",
                priority = int.MaxValue,
                title = "Sample Note / 示例信息",
                content = "This is a <b>sample note.</b>\r\n这是一个<b>示例信息。</b>\r\n\r\n" +
                          "You can input some information here and then upload this asset to a version control system to share the information within your team.\r\n" +
                          "可以在这里输入一些信息，然后将此资产上传到版本控制系统，来在团队中分享信息。\r\n\r\n" +
                          "The information content supports <color=green>rich text</color>, see reference:\r\n" +
                          "信息内容支持<color=green>富文本</color>，参考：\r\n\r\n" +
#if UNITY_2022_3_OR_NEWER
                          "    <a href=\"https://docs.unity3d.com/Manual/UIE-supported-tags.html\">Supported rich text tags</a>",
#else
                          "    <i>https://docs.unity3d.com/Manual/UIE-supported-tags.html</i>",
#endif
                contentHistory = {
                    new NoteHistory(638523554912390682, "This is a <color=blue>historical version</color> sample.\r\n这是一个<color=blue>历史版本</color>示例。")
                },
            }
        };


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