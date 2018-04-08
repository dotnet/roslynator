// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator.CodeFixes
{
    public sealed class CodeFixSettings
    {
        public CodeFixSettings()
        {
            DisabledCodeFixes = new CodeFixIdentifierSet();
        }

        public static CodeFixSettings Current { get; } = new CodeFixSettings();

        public CodeFixIdentifierSet DisabledCodeFixes { get; }

        public void Reset()
        {
            DisabledCodeFixes.Clear();
        }

        public bool IsCodeFixEnabled(string id)
        {
            return !DisabledCodeFixes.Contains(id);
        }

        public bool IsAnyCodeFixEnabled(string id, string id2)
        {
            return IsCodeFixEnabled(id)
                || IsCodeFixEnabled(id2);
        }

        public bool IsAnyCodeFixEnabled(string id, string id2, string id3)
        {
            return IsCodeFixEnabled(id)
                || IsCodeFixEnabled(id2)
                || IsCodeFixEnabled(id3);
        }

        public bool IsAnyCodeFixEnabled(string id, string id2, string id3, string id4)
        {
            return IsCodeFixEnabled(id)
                || IsCodeFixEnabled(id2)
                || IsCodeFixEnabled(id3)
                || IsCodeFixEnabled(id4);
        }

        public bool IsAnyCodeFixEnabled(string id, string id2, string id3, string id4, string id5)
        {
            return IsCodeFixEnabled(id)
                || IsCodeFixEnabled(id2)
                || IsCodeFixEnabled(id3)
                || IsCodeFixEnabled(id4)
                || IsCodeFixEnabled(id5);
        }

        public bool IsAnyCodeFixEnabled(
            string id,
            string id2,
            string id3,
            string id4,
            string id5,
            string id6)
        {
            return IsCodeFixEnabled(id)
                || IsCodeFixEnabled(id2)
                || IsCodeFixEnabled(id3)
                || IsCodeFixEnabled(id4)
                || IsCodeFixEnabled(id5)
                || IsCodeFixEnabled(id6);
        }

        public void DisableCodeFix(string id)
        {
#if DEBUG
            Debug.WriteLineIf(DisabledCodeFixes.Add(id), $"code fix {id} disabled");
#else
            DisabledCodeFixes.Add(id);
#endif
        }

        public void EnableCodeFix(string id)
        {
#if DEBUG
            Debug.WriteLineIf(DisabledCodeFixes.Remove(id), $"code fix {id} enabled");
#else
            DisabledCodeFixes.Remove(id);
#endif
        }

        public void SetCodeFix(string id, bool isEnabled)
        {
            if (isEnabled)
            {
                EnableCodeFix(id);
            }
            else
            {
                DisableCodeFix(id);
            }
        }
    }
}
