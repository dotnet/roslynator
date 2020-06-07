// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Roslynator.Configuration;

namespace Roslynator
{
    internal sealed class RefactoringSettings : CodeAnalysisSettings<string>
    {
        public static RefactoringSettings Current { get; } = LoadSettings();

        private static RefactoringSettings LoadSettings()
        {
            var settings = new RefactoringSettings();

            settings.Reset();

            return settings;
        }

        public bool PrefixFieldIdentifierWithUnderscore { get; set; }

        protected override void SetValues(CodeAnalysisConfiguration configuration)
        {
            if (configuration == null)
                return;

            foreach (KeyValuePair<string, bool> kvp in configuration.Refactorings)
                Set(kvp.Key, kvp.Value);

            PrefixFieldIdentifierWithUnderscore = configuration.PrefixFieldIdentifierWithUnderscore;
        }
    }
}
