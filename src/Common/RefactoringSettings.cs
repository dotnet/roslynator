// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator
{
    public sealed class RefactoringSettings
    {
        public RefactoringSettings()
        {
            DisabledRefactorings = new RefactoringIdentifierSet();
        }

        public static RefactoringSettings Current { get; } = new RefactoringSettings();

        public RefactoringIdentifierSet DisabledRefactorings { get; set; }

        public bool PrefixFieldIdentifierWithUnderscore { get; set; } = true;

        public void Reset()
        {
            PrefixFieldIdentifierWithUnderscore = true;
            DisabledRefactorings.Clear();
        }

        public bool IsRefactoringEnabled(string id)
        {
            return !DisabledRefactorings.Contains(id);
        }

        public bool IsAnyRefactoringEnabled(string id, string id2)
        {
            return IsRefactoringEnabled(id)
                || IsRefactoringEnabled(id2);
        }

        public bool IsAnyRefactoringEnabled(string id, string id2, string id3)
        {
            return IsRefactoringEnabled(id)
                || IsRefactoringEnabled(id2)
                || IsRefactoringEnabled(id3);
        }

        public bool IsAnyRefactoringEnabled(string id, string id2, string id3, string id4)
        {
            return IsRefactoringEnabled(id)
                || IsRefactoringEnabled(id2)
                || IsRefactoringEnabled(id3)
                || IsRefactoringEnabled(id4);
        }

        public bool IsAnyRefactoringEnabled(string id, string id2, string id3, string id4, string id5)
        {
            return IsRefactoringEnabled(id)
                || IsRefactoringEnabled(id2)
                || IsRefactoringEnabled(id3)
                || IsRefactoringEnabled(id4)
                || IsRefactoringEnabled(id5);
        }

        public bool IsAnyRefactoringEnabled(string id, string id2, string id3, string id4, string id5, string id6)
        {
            return IsRefactoringEnabled(id)
                || IsRefactoringEnabled(id2)
                || IsRefactoringEnabled(id3)
                || IsRefactoringEnabled(id4)
                || IsRefactoringEnabled(id5)
                || IsRefactoringEnabled(id6);
        }

        public void DisableRefactoring(string id)
        {
#if DEBUG
            Debug.WriteLineIf(DisabledRefactorings.Add(id), $"refactoring {id} disabled");
#else
            DisabledRefactorings.Add(id);
#endif
        }

        public void EnableRefactoring(string id)
        {
#if DEBUG
            Debug.WriteLineIf(DisabledRefactorings.Remove(id), $"refactoring {id} enabled");
#else
            DisabledRefactorings.Remove(id);
#endif
        }

        public void SetRefactoring(string id, bool isEnabled)
        {
            if (isEnabled)
            {
                EnableRefactoring(id);
            }
            else
            {
                DisableRefactoring(id);
            }
        }
    }
}
