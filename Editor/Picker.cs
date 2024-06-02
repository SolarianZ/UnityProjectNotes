using System;
using System.Collections.Generic;
using UnityEditor.Search;

namespace GBG.ProjectNotes.Editor
{
    internal static class Picker
    {
        public const string Pattern_Title = "title: ";
        public const string Pattern_Content = "content: ";
        public const string Pattern_Author = "author: ";


        public static void Pick(List<NoteEntry> notes, string rawPattern)
        {
            if (string.IsNullOrWhiteSpace(rawPattern))
            {
                foreach (NoteEntry note in notes)
                {
                    note.displayPriority = note.priority;
                }
                return;
            }

            rawPattern = rawPattern.Trim();

            if (rawPattern.StartsWith(Pattern_Title, StringComparison.OrdinalIgnoreCase))
            {
                string pattern = rawPattern.Substring(Pattern_Title.Length);
                PickInTitleAppendMode(notes, pattern);
                return;
            }

            if (rawPattern.StartsWith(Pattern_Content, StringComparison.OrdinalIgnoreCase))
            {
                string pattern = rawPattern.Substring(Pattern_Content.Length);
                PickInContentAppendMode(notes, pattern);
                return;
            }

            if (rawPattern.StartsWith(Pattern_Author, StringComparison.OrdinalIgnoreCase))
            {
                string pattern = rawPattern.Substring(Pattern_Author.Length);
                PickInAuthorAppendMode(notes, pattern);
                return;
            }

            PickInNoteAppendMode(notes, rawPattern);
        }

        public static void PickInNoteAppendMode(List<NoteEntry> notes, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                foreach (NoteEntry note in notes)
                {
                    note.displayPriority = note.priority;
                }
                return;
            }

            for (int i = notes.Count - 1; i >= 0; i--)
            {
                NoteEntry note = notes[i];
                bool match = false;
                long maxScore = int.MinValue;
                long score = 0;
                if (FuzzySearch.FuzzyMatch(pattern, note.title, ref score))
                {
                    match = true;
                    if (score > maxScore) { maxScore = score; }
                }
                if (FuzzySearch.FuzzyMatch(pattern, note.content, ref score))
                {
                    match = true;
                    if (score > maxScore) { maxScore = score; }
                }
                if (FuzzySearch.FuzzyMatch(pattern, note.author, ref score))
                {
                    match = true;
                    if (score > maxScore) { maxScore = score; }
                }

                if (match)
                {
                    note.displayPriority = maxScore;
                }
                else
                {
                    notes.RemoveAt(i);
                }
            }
        }

        public static void PickInTitleAppendMode(List<NoteEntry> notes, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                foreach (NoteEntry note in notes)
                {
                    note.displayPriority = note.priority;
                }
                return;
            }

            for (int i = notes.Count - 1; i >= 0; i--)
            {
                NoteEntry note = notes[i];
                long score = 0;
                if (FuzzySearch.FuzzyMatch(pattern, note.title, ref score))
                {
                    note.displayPriority = score;
                }
                else
                {
                    notes.RemoveAt(i);
                }
            }
        }

        public static void PickInContentAppendMode(List<NoteEntry> notes, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                foreach (NoteEntry note in notes)
                {
                    note.displayPriority = note.priority;
                }
                return;
            }

            for (int i = notes.Count - 1; i >= 0; i--)
            {
                NoteEntry note = notes[i];
                long score = 0;
                if (FuzzySearch.FuzzyMatch(pattern, note.content, ref score))
                {
                    note.displayPriority = score;
                }
                else
                {
                    notes.RemoveAt(i);
                }
            }
        }

        public static void PickInAuthorAppendMode(List<NoteEntry> notes, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                foreach (NoteEntry note in notes)
                {
                    note.displayPriority = note.priority;
                }
                return;
            }

            for (int i = notes.Count - 1; i >= 0; i--)
            {
                NoteEntry note = notes[i];
                long score = 0;
                if (FuzzySearch.FuzzyMatch(pattern, note.author, ref score))
                {
                    note.displayPriority = score;
                }
                else
                {
                    notes.RemoveAt(i);
                }
            }
        }
    }
}