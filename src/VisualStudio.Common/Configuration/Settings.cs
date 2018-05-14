// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Roslynator.CodeFixes;

namespace Roslynator.Configuration
{
    [SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "<Pending>")]
    public class Settings
    {
        public bool PrefixFieldIdentifierWithUnderscore { get; set; } = true;

        public Dictionary<string, bool> Refactorings { get; } = new Dictionary<string, bool>(StringComparer.Ordinal);

        public Dictionary<string, bool> CodeFixes { get; } = new Dictionary<string, bool>(StringComparer.Ordinal);

        public virtual void Update(Settings settings)
        {
            PrefixFieldIdentifierWithUnderscore = settings.PrefixFieldIdentifierWithUnderscore;

            Refactorings.Clear();

            foreach (KeyValuePair<string, bool> kvp in settings.Refactorings)
                Refactorings[kvp.Key] = kvp.Value;

            CodeFixes.Clear();

            foreach (KeyValuePair<string, bool> kvp in settings.CodeFixes)
                CodeFixes[kvp.Key] = kvp.Value;
        }

        public void ApplyTo(RefactoringSettings settings)
        {
            settings.PrefixFieldIdentifierWithUnderscore = PrefixFieldIdentifierWithUnderscore;

            foreach (KeyValuePair<string, bool> kvp in Refactorings)
                settings.SetRefactoring(kvp.Key, kvp.Value);
        }

        public void ApplyTo(CodeFixSettings settings)
        {
            foreach (KeyValuePair<string, bool> kvp in CodeFixes)
                settings.SetCodeFix(kvp.Key, kvp.Value);
        }
    }
}
