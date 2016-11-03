// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings
{
    public sealed class RefactoringSettings
    {
        private RefactoringIdentifierSet _disabledRefactorings;

        public RefactoringSettings()
        {
            _disabledRefactorings = new RefactoringIdentifierSet();
        }

        public bool PrefixFieldIdentifierWithUnderscore { get; set; } = true;

        public bool IsRefactoringEnabled(string identifier)
        {
            return !_disabledRefactorings.Contains(identifier);
        }

        public bool IsAnyRefactoringEnabled(string identifier, string identifier2)
        {
            return IsRefactoringEnabled(identifier)
                || IsRefactoringEnabled(identifier2);
        }

        public bool IsAnyRefactoringEnabled(string identifier, string identifier2, string identifier3)
        {
            return IsRefactoringEnabled(identifier)
                || IsRefactoringEnabled(identifier2)
                || IsRefactoringEnabled(identifier3);
        }

        public bool IsAnyRefactoringEnabled(string identifier, string identifier2, string identifier3, string identifier4)
        {
            return IsRefactoringEnabled(identifier)
                || IsRefactoringEnabled(identifier2)
                || IsRefactoringEnabled(identifier3)
                || IsRefactoringEnabled(identifier4);
        }

        public bool IsAnyRefactoringEnabled(string identifier, string identifier2, string identifier3, string identifier4, string identifier5)
        {
            return IsRefactoringEnabled(identifier)
                || IsRefactoringEnabled(identifier2)
                || IsRefactoringEnabled(identifier3)
                || IsRefactoringEnabled(identifier4)
                || IsRefactoringEnabled(identifier5);
        }

        public bool IsAnyRefactoringEnabled(
            string identifier,
            string identifier2,
            string identifier3,
            string identifier4,
            string identifier5,
            string identifier6)
        {
            return IsRefactoringEnabled(identifier)
                || IsRefactoringEnabled(identifier2)
                || IsRefactoringEnabled(identifier3)
                || IsRefactoringEnabled(identifier4)
                || IsRefactoringEnabled(identifier5)
                || IsRefactoringEnabled(identifier6);
        }

        public void SetIsRefactoringEnabled(string identifier, bool isEnabled)
        {
            if (isEnabled)
            {
                _disabledRefactorings.Remove(identifier);
            }
            else
            {
                _disabledRefactorings.Add(identifier);
            }
        }
    }
}
