// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
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

        public bool IsAnyRefactoringEnabled(params string[] identifiers)
        {
            return identifiers.Any(IsRefactoringEnabled);
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
