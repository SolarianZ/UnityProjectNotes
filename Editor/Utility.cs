using System;
using System.Collections.Generic;

namespace GBG.ProjectNotes.Editor
{
    public static class Utility
    {
        public static string NewGuid() => Guid.NewGuid().ToString();
        public static long NewTimestamp() => DateTime.Now.Ticks;


        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";
        public static float ButtonBorderRadius = 2;

        public static string FormatTimestamp(long timestamp)
        {
            return new DateTime(timestamp).ToString(DateTimeFormat);
        }

        public static void CollectHistoryTimestamps(this NoteEntry note, List<long> timestamps)
        {
            timestamps.Clear();
            if (note == null)
            {
                //timestamps.Add(0);
                return;
            }

            timestamps.Add(note.timestamp);
            foreach (NoteHistory history in note.contentHistory)
            {
                timestamps.Add(history.timestamp);
            }
        }

        public static bool HasUnreadNotesInCategory(string category)
        {
            ProjectNotesSettings settings = ProjectNotesSettings.instance;
            if (!settings)
            {
                return false;
            }

            category = category?.Trim();
            if (category == ProjectNotesSettings.CategoryAll)
            {
                foreach (NoteEntry note in settings.Notes)
                {
                    if (ProjectNotesLocalCache.instance.IsUnread(note.GetKey()))
                    {
                        return true;
                    }
                }
            }

            foreach (NoteEntry note in settings.Notes)
            {
                if (note.categoryTrimmed == category &&
                    ProjectNotesLocalCache.instance.IsUnread(note.GetKey()))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<CategoryInfo> CollectCategoriesOrderByMaxPriority(this ProjectNotesSettings settings, bool addCategoryAll = true)
        {
            Dictionary<string, CategoryInfo> categoryInfoDict = new Dictionary<string, CategoryInfo>();
            bool hasUnreadNotes = false;
            foreach (NoteEntry note in settings.Notes)
            {
                string category = note.categoryTrimmed;
                if (category == ProjectNotesSettings.CategoryAll || string.IsNullOrEmpty(category))
                {
                    if (!hasUnreadNotes && addCategoryAll)
                    {
                        hasUnreadNotes = ProjectNotesLocalCache.instance.IsUnread(note.GetKey());
                    }
                    continue;
                }

                if (!categoryInfoDict.TryGetValue(category, out CategoryInfo categoryInfo))
                {
                    bool noteUnread = ProjectNotesLocalCache.instance.IsUnread(note.GetKey());
                    hasUnreadNotes |= noteUnread;
                    categoryInfo = new CategoryInfo
                    {
                        category = category,
                        maxPriority = note.priority,
                        hasUnreadNotes = noteUnread,
                    };
                    categoryInfoDict[category] = categoryInfo;

                    continue;
                }

                bool infoChanged = false;
                if (categoryInfo.maxPriority < note.priority)
                {
                    categoryInfo.maxPriority = note.priority;
                    infoChanged = true;
                }
                if (!categoryInfo.hasUnreadNotes && ProjectNotesLocalCache.instance.IsUnread(note.GetKey()))
                {
                    categoryInfo.hasUnreadNotes = true;
                    infoChanged = true;
                }
                if (infoChanged)
                {
                    categoryInfoDict[category] = categoryInfo;
                }
            }

            List<CategoryInfo> categoryInfos = new List<CategoryInfo>(categoryInfoDict.Count + 1);
            if (addCategoryAll)
            {
                categoryInfos.Add(new CategoryInfo
                {
                    category = ProjectNotesSettings.CategoryAll,
                    maxPriority = int.MaxValue,
                    hasUnreadNotes = hasUnreadNotes,
                });
            }

            foreach (CategoryInfo newCategoryInfo in categoryInfoDict.Values)
            {
                bool added = false;
                for (int i = categoryInfos.Count - 1; i >= 0; i--)
                {
                    CategoryInfo categoryInfo = categoryInfos[i];
                    if (categoryInfo.maxPriority >= newCategoryInfo.maxPriority)
                    {
                        categoryInfos.Insert(i + 1, newCategoryInfo);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    categoryInfos.Insert(0, newCategoryInfo);
                }
            }

            return categoryInfos;
        }


        public static int MinTitleLength = 4;
        public static int MinContentLength = 12;

        public static bool IsNiceTitle(string title, out string alert)
        {
            if (title.Length < MinTitleLength)
            {
                alert = $"Short title may lack recognizability.";
                return false;
            }

            alert = null;
            return true;
        }

        public static bool IsNiceContent(string content, out string alert)
        {
            if (content.Length < MinContentLength)
            {
                alert = $"Short content may be difficult to understand.";
                return false;
            }

            alert = null;
            return true;
        }


        public const string RedDotIconName = "redLight"; // "winbtn_mac_close"
    }
}