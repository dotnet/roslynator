// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Roslynator.CodeFixes;
using Roslynator.VisualStudio;

namespace Roslynator.Configuration
{
    public sealed class SettingsManager
    {
        private SettingsManager()
        {
        }

        public bool UseConfigFile { get; set; }

        public static SettingsManager Instance { get; } = new SettingsManager();

        public Settings VisualStudioSettings { get; } = new Settings();

        public Settings ConfigFileSettings { get; set; }

        public void UpdateVisualStudioSettings(GeneralOptionsPage generalOptionsPage)
        {
            UseConfigFile = generalOptionsPage.UseConfigFile;

            VisualStudioSettings.PrefixFieldIdentifierWithUnderscore = generalOptionsPage.PrefixFieldIdentifierWithUnderscore;
        }

        public void UpdateVisualStudioSettings(RefactoringsOptionsPage refactoringsOptionsPage)
        {
            VisualStudioSettings.Refactorings.Clear();

            foreach (string id in refactoringsOptionsPage.GetDisabledItems())
                VisualStudioSettings.Refactorings[id] = false;
        }

        public void UpdateVisualStudioSettings(CodeFixesOptionsPage codeFixOptionsPage)
        {
            VisualStudioSettings.CodeFixes.Clear();

            foreach (string id in codeFixOptionsPage.GetDisabledItems())
                VisualStudioSettings.CodeFixes[id] = false;
        }

        public void UpdateVisualStudioSettings(GlobalSuppressionsOptionsPage globalSuppressionsOptionsPage)
        {
            VisualStudioSettings.GlobalSuppressions.Clear();

            foreach (string id in globalSuppressionsOptionsPage.GetDisabledItems())
                VisualStudioSettings.GlobalSuppressions.Add(id);
        }

        public void ApplyTo(AnalyzerSettings analyzerSettings)
        {
            analyzerSettings.Reset();

            Apply(VisualStudioSettings);

            if (UseConfigFile)
                Apply(ConfigFileSettings);

            void Apply(Settings settings)
            {
                if (settings != null)
                {
                    foreach (string id in settings.GlobalSuppressions)
                        analyzerSettings.Disable(id);
                }
            }
        }

        internal void ApplyTo(RefactoringSettings refactoringSettings)
        {
            refactoringSettings.Reset();

            Apply(VisualStudioSettings);

            if (UseConfigFile)
                Apply(ConfigFileSettings);

            void Apply(Settings settings)
            {
                if (settings != null)
                {
                    refactoringSettings.PrefixFieldIdentifierWithUnderscore = settings.PrefixFieldIdentifierWithUnderscore;
                    refactoringSettings.Set(settings.Refactorings);
                }
            }
        }

        public void ApplyTo(CodeFixSettings codeFixSettings)
        {
            codeFixSettings.Reset();

            Apply(VisualStudioSettings);

            if (UseConfigFile)
                Apply(ConfigFileSettings);

            void Apply(Settings settings)
            {
                if (settings != null)
                {
                    foreach (KeyValuePair<string, bool> kvp in settings.CodeFixes)
                    {
                        if (CodeFixIdentifier.TryParse(kvp.Key, out CodeFixIdentifier codeFixIdentifier))
                        {
                            codeFixSettings.Set(codeFixIdentifier, kvp.Value);
                        }
                    }
                }
            }
        }
    }
}
